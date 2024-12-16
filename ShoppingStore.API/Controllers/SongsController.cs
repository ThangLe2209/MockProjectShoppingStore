using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.API.Services;
using ShoppingStore.Model;

namespace ShoppingStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : Controller
    {
        private readonly ISongRepository _songRepository;
        private readonly IMapper _mapper;
        const int maxSongPageSize = 4;
        public SongsController(ISongRepository songRepository, IMapper mapper)
        {
            _songRepository = songRepository ?? throw new ArgumentNullException(nameof(songRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongDto>>> GetSongs()
        {
            var songEntities = await _songRepository.GetSongsAsync();
            var result = _mapper.Map<IEnumerable<SongDto>>(songEntities);
            return Ok(result);
        }

        [HttpGet("{songId}", Name = "GetSongById")]
        public async Task<IActionResult> GetSongById(Guid songId)
        {
            var song = await _songRepository.GetSongByIdAsync(songId);
            if (song == null)
            {
                return NotFound("Song not existed");
            }

            return Ok(_mapper.Map<SongDto>(song));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSong([FromForm] SongForCreationDto song) // get FromForm because mapping MultiplePartDatatype
        {
            //var a = 10;
            var checkSongName = await _songRepository.CheckSongNameAsync(song.Name);

            if (checkSongName)
            {
                return BadRequest("Song name already exist in database");
            }

            var checkSongUpload = _songRepository.CheckSongUpload(song.SongUpload);
            if (checkSongUpload) return BadRequest("Song Upload already exist in database");
            song.Song = await _songRepository.AddSongAsync(song.SongUpload);

            if (song.ImageUrl != null)
            {
                var checkSongImageUrl = await _songRepository.CheckSongImageUrlAsync(song.ImageUrl);
                if (checkSongImageUrl) return BadRequest("Song Image Url already exist in database");
                song.Image = song.ImageUrl;
            }
            else if (song.ImageUpload != null)
            {
                var checkSongImageUpload = _songRepository.CheckSongImageUpload(song.ImageUpload);
                if (checkSongImageUpload) return BadRequest("Song Image Upload already exist in database");
                song.Image = await _songRepository.AddSongImageAsync(song.ImageUpload);
            }

            var finalSong = _mapper.Map<SongModel>(song);
            //var a = 10;
            _songRepository.AddSong(finalSong);
            //var b = 10;
            await _songRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
            //var c = 20;
            var createdSongToReturn = _mapper.Map<SongDto>(finalSong);
            //var a = 10;
            return CreatedAtRoute("GetSongById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
                new
                {
                    songId = createdSongToReturn.Id,
                } // value API Get line 89 need - Api get specific product by id
                , createdSongToReturn); // final Data (include in response body)
        }

        [HttpPut("{songId}")]
        public async Task<IActionResult> UpdateSong(Guid songId, [FromForm] SongForEditDto updatedSong)
        {
            try
            {
                SongModel currentSong = await _songRepository.GetSongByIdAsync(songId);
                if (currentSong == null)
                {
                    //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound("Song not existed");
                }

                var checkSongName = await _songRepository.CheckSongNameUpdateAsync(songId, updatedSong.Name);

                if (checkSongName)
                {
                    return BadRequest("Song name already exist in database");
                }

                if (updatedSong.SongUpload != null && !currentSong.Song.Contains(updatedSong.SongUpload.FileName))
                {
                    var checkSongUpload = _songRepository.CheckSongUpload(updatedSong.SongUpload);
                    if (checkSongUpload) return BadRequest("Song Upload already exist in database");
                    _songRepository.DeleteOldSong(currentSong.Song);
                    currentSong.Song = await _songRepository.AddSongAsync(updatedSong.SongUpload);
                }

                if (updatedSong.ImageUrl != null)
                {
                    if (updatedSong.ImageUrl != currentSong.Image)
                    {
                        var checkSongImageUrl = await _songRepository.CheckSongImageUrlAsync(updatedSong.ImageUrl);
                        if (checkSongImageUrl) return BadRequest("Song Image Url already exist in database");
                        if (currentSong.Image != "noimage.jpg") _songRepository.DeleteOldSongImage(currentSong.Image);
                        currentSong.Image = updatedSong.ImageUrl;
                    }
                }
                else if (updatedSong.ImageUpload != null && !currentSong.Image.Contains(updatedSong.ImageUpload.FileName))
                {
                    var checkSongImageUpload = _songRepository.CheckSongImageUpload(updatedSong.ImageUpload);
                    if (checkSongImageUpload) return BadRequest("Song Image Upload already exist in database");
                    if (currentSong.Image != "noimage.jpg") _songRepository.DeleteOldSongImage(currentSong.Image);
                    currentSong.Image = await _songRepository.AddSongImageAsync(updatedSong.ImageUpload);
                }

                //var a = 10;
                _mapper.Map(updatedSong, currentSong); // source, dest => use mapper like this will override data from source to dest
                //var b = 11;
                await _songRepository.SaveChangesAsync();
                //var c = 20;
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{songId}")]
        public async Task<ActionResult> DeleteSong(Guid songId)
        {
            try
            {
                SongModel currentSong = await _songRepository.GetSongByIdAsync(songId);
                var songImagePath = currentSong.Image;
                var songPath = currentSong.Song;
                if (currentSong == null)
                {
                    //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound("Song not existed");
                }

                _songRepository.DeleteSong(currentSong);
                await _songRepository.SaveChangesAsync();
                if (songImagePath != "noimage.jpg") _songRepository.DeleteOldSongImage(songImagePath);
                if (songImagePath != String.Empty) _songRepository.DeleteOldSong(songPath); // String.Empty = ""

                //_mailService.Send("Point of interest deleted.",
                //    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
