/**
 * 1. Render songs
 * 2. Scroll top
 * 3. Play/ pause/ seek
 * 4. CD rotate
 * 5. Next /prev
 * 6. Random
 * 7. Next / Repeat when ended
 * 8. Active song
 * 9. Scroll active song into view
 * 10. Play song when clicked
*/

const $js = document.querySelector.bind(document);

const PLAYER_STORAGE_KEY = 'F8_PLAYER';

const player = $js('.player')
const cd = $js('.cd');
const heading = $js('header>h2');
const cdThumb = $js('.cd-thumb');
const audio = $js('#audio');
const playBtn = $js('.btn-toggle-play');
const progress = $js('#progress');
const prevBtn = $js('.btn-prev');
const nextBtn = $js('.btn-next');
const randomBtn = $js('.btn-random');
const repeatBtn = $js('.btn-repeat');
const playlist = $js('.playlist');
const audioVolumeBar = $js('#audio-volume')
const currentVolume = $js('.current-volume')
const timer = $js('.time-remain')

const app = {
    currentIndex: 0,
    isPlaying: false,
    isRandom: false,
    isRepeat: false,
    config: JSON.parse(localStorage.getItem(PLAYER_STORAGE_KEY)) || {},
    songs: [
        {
            name: 'Song',
            singer: 'NightCore',
            path: './songs/song1.mp3',
            img: './images/song/song1.jpg',
        },
        {
            name: 'Song1',
            singer: 'NightCore1',
            path: './songs/song2.mp3',
            img: './images/song/song2.jpg',
        },
        {
            name: 'Song2',
            singer: 'NightCore2',
            path: './songs/song3.mp3',
            img: './images/song/song3.jpg',
        },
        {
            name: 'Song3',
            singer: 'NightCore3',
            path: './songs/song4.mp3',
            img: './images/song/song4.jpg',
        },
        {
            name: 'Song4',
            singer: 'NightCore4',
            path: './songs/song5.mp3',
            img: './images/song/song5.jpg',
        },
        {
            name: 'Song5',
            singer: 'NightCore5',
            path: './songs/song6.mp3',
            img: './images/song/song6.jpg',
        },
        {
            name: 'Song6',
            singer: 'NightCore6',
            path: './songs/song7.mp3',
            img: './images/song/song7.jpg',
        },
        {
            name: 'Song7',
            singer: 'NightCore7',
            path: './songs/song8.mp3',
            img: './images/song/song8.jpg',
        },
        {
            name: 'Song8',
            singer: 'NightCore8',
            path: './songs/song9.mp3',
            img: './images/song/song9.jpg',
        },
        {
            name: 'Song9',
            singer: 'NightCore9',
            path: './songs/song10.mp3',
            img: './images/song/song10.jpg',
        },
        {
            name: 'Song10',
            singer: 'NightCore10',
            path: './songs/song11.mp3',
            img: './images/song/song11.jpg',
        },
    ],
    setConfig(key, value) {
        this.config[key] = value;
        localStorage.setItem(PLAYER_STORAGE_KEY, JSON.stringify(this.config));
    },
    //render: function () {
    //    const htmls = this.songs.map((song, index) => {
    //        return `
    //                        <div class="song songItem${index} ${index === this.currentIndex ? 'active' : ''}" data-index=${index}>
    //                            <div class="thumb"
    //                                style="background-image: url(${song.img})">
    //                            </div>
    //                            <div class="body">
    //                                <h3 class="title">${song.name}</h3>
    //                                <p class="author">${song.singer}</p>
    //                            </div>
    //                            <div class="option">
    //                                <i class="fas fa-ellipsis-h"></i>
    //                            </div>
    //                        </div>

    //                        `
    //    })
    //    playlist.innerHTML = htmls.join('');
    //},
    defineProperties: function () {
        Object.defineProperty(this, 'currentSong', {
            get: function () {
                return this.songs[this.currentIndex];
            }
        })
    },
    handleEvents: function () {
        const _this = this;
        const cdWidth = cd.offsetWidth;
        audio.volume = 0.5;

        // Xử lí CD quay / dừng
        const cdThumbAnimate = cdThumb.animate([
            { transform: 'rotate(360deg)' }
        ], {
            duration: 10000, // 10 seconds
            iterations: Infinity
        })
        cdThumbAnimate.pause();

        // Xử lí khi click play
        playBtn.onclick = function () {
            if (_this.isPlaying) {
                audio.pause();
            } else {
                audio.play();
            }
        }

        // Khi song được play
        audio.onplay = function () {
            // console.log(this);
            _this.isPlaying = true;
            player.classList.add('playing');
            cdThumbAnimate.play();
            timer.style.display = 'block';
            _this.setConfig('songNow', _this.currentIndex);
        }

        // Khi song bị pause
        audio.onpause = function () {
            _this.isPlaying = false;
            player.classList.remove('playing');
            cdThumbAnimate.pause();
        }

        // Khi tiến độ bài hát thay đổi
        audio.ontimeupdate = function () {
            if (audio.duration) {
                const progressPercent = Math.floor(audio.currentTime / audio.duration * 100)
                progress.value = progressPercent;

                const timeRemain = audio.duration - audio.currentTime
                let timeRemainAsMinute
                if (Math.floor(timeRemain % 60) < 10) {
                    timeRemainAsMinute = (timeRemain - (timeRemain % 60)) / 60 + ':0' + Math.floor(timeRemain % 60)
                    timer.textContent = timeRemainAsMinute
                } else {
                    timeRemainAsMinute = (timeRemain - (timeRemain % 60)) / 60 + ':' + Math.floor(timeRemain % 60)
                    timer.textContent = timeRemainAsMinute

                }
            }
        }

        // Xử lí khi tua song
        progress.oninput = function (e) {
            const seekTime = (e.target.value / 100) * audio.duration; // this.value <=> progress.value <=> e.target.value
            audio.currentTime = seekTime;
        }

        // Khi next song
        nextBtn.onclick = function () {
            // $(`.songItem${_this.currentIndex}`).classList.remove('active');
            //$js(`[data-index='${_this.currentIndex}']`).classList.remove('active');
            if (_this.isRandom) {
                _this.playRandomSong();
            }
            else {
                _this.nextSong();
            }
            // $(`.songItem${_this.currentIndex}`).classList.add('active');
            //$js(`[data-index='${_this.currentIndex}']`).classList.add('active');
            audio.play();
            //_this.scrollToActiveSong();
        }

        // Khi prev song
        prevBtn.onclick = function () {
            //$js(`.songItem${_this.currentIndex}`).classList.remove('active');
            if (_this.isRandom) {
                _this.playRandomSong();
            }
            else {
                _this.prevSong();
            }
            //$js(`.songItem${_this.currentIndex}`).classList.add('active');
            audio.play();
            //_this.scrollToActiveSong();
        }

        // Xử lí bật/ tắt random song
        randomBtn.onclick = function () {
            _this.isRandom = !_this.isRandom;
            _this.setConfig('isRandom', _this.isRandom);
            randomBtn.classList.toggle('active', _this.isRandom);
        }

        // Xử lí lặp lại một song
        repeatBtn.onclick = function () {
            _this.isRepeat = !_this.isRepeat;
            _this.setConfig('isRepeat', _this.isRepeat);
            repeatBtn.classList.toggle('active', _this.isRepeat);
        }

        // Xử lí next song khi audio ended
        audio.onended = function () {
            if (_this.isRepeat) {
                audio.play();
            }
            else nextBtn.click();
        }

        // Xử lý thanh âm lượng
        audioVolumeBar.oninput = function (e) {
            audio.volume = e.target.value / audioVolumeBar.max
            // console.log(audio.volume);
            currentVolume.textContent = e.target.value + '%';
        }
    },
    loadCurrentSong: function () {
        heading.textContent = this.currentSong.name;
        cdThumb.style.backgroundImage = `url(${this.currentSong.img})`;
        audio.src = this.currentSong.path;
    },
    loadConfig() {
        // Object.assign(this, this.config);
        this.isRandom = this.config.isRandom || false;
        this.isRepeat = this.config.isRepeat || false;
        this.currentIndex = this.config.songNow || 0;
    },
    nextSong: function () {
        this.currentIndex++;
        if (this.currentIndex >= this.songs.length) {
            this.currentIndex = 0;
        }
        this.loadCurrentSong();
    },
    prevSong: function () {
        this.currentIndex--;
        if (this.currentIndex < 0) {
            this.currentIndex = this.songs.length - 1;
        }
        this.loadCurrentSong();
    },
    playRandomSong: function () {
        let newIndex;
        do {
            newIndex = Math.floor(Math.random() * this.songs.length);
        } while (newIndex === this.currentIndex)
        this.currentIndex = newIndex;
        this.loadCurrentSong();
    },
    start: function () {
        // Gán cấu hình từ config vào ứng dụng(Object)
        this.loadConfig();

        // Định nghĩa các thuộc tính cho object
        this.defineProperties();

        // Lắng nghe / xử lí các sự kiện (DOM events)
        this.handleEvents();

        // Tải thông tin bài hát đầu tiên vào UI khi chạy ứng dụng
        this.loadCurrentSong();

        // Render playlist
        //this.render();

        // Hiện thị trạng thái ban đầu của button repeat và random
        randomBtn.classList.toggle('active', this.isRandom);
        repeatBtn.classList.toggle('active', this.isRepeat);

    }
};

app.start();

if ((window.location.origin + "/" === window.location.href)) {
    // Index (home) page
    if (sessionStorage.getItem('isLoad') === null) {
        $js('.small-song-btn').style.display = 'none';
        const elementToFadeInAndOut = $js('.elementToFadeInAndOut');
        elementToFadeInAndOut.style.animation = "slide-right 4s 1 forwards";

        elementToFadeInAndOut.addEventListener("animationend", () => {
            $js('.small-song-btn').style.display = 'block';
            sessionStorage.setItem('isLoad', true)
        });
    }
}


// click icon menu in _SongPartial.cshtml
const handleClickSongMenu = () => {
    // document.querySelector('.elementToFadeInAndOut').style.right = '280px';
    var opacity = parseInt(
        window.getComputedStyle($js('.elementToFadeInAndOut')).getPropertyValue('opacity'),
    );
    if (opacity === 0) {
        $js('.elementToFadeInAndOut').classList.remove('animation-out');
        $js('.elementToFadeInAndOut').classList.add('animation-in');
    } else {
        $js('.elementToFadeInAndOut').classList.remove('animation-in');
        $js('.elementToFadeInAndOut').classList.add('animation-out');
    }
    // document.querySelector('.elementToFadeInAndOut').style.opacity = 1;
};

