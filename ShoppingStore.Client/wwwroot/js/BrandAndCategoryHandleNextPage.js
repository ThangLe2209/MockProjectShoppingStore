function HandleNextBrandMenu(input, totalPageCount, slugQueryString) {
    $(".brands_products").load("/Brand/NextBrandViewModelMenu", { input: input, totalPageCount, slugQueryString })
}

function HandleNextCategoryMenu(input, totalPageCount, slugQueryString) {
    $(".panel-group.category-products").load("/Category/NextCategoryViewModelMenu", { input: input, totalPageCount, slugQueryString })
}


function HandleNextRatingPage({ input, productId, totalPageCount }) {
    $("#rating-product-table").load("/Product/NextRatingViewModelPage", { productId: productId, input: input, totalPageCount })
}