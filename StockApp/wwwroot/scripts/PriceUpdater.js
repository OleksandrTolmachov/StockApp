const intervalID = setInterval(updatePrice, 2000, 'http://localhost:5266/api/StockApi/Price');

async function getObjFromApi(url) {
    let response = await fetch(url)
    let data = await response.json()
    return data
}

async function updatePrice(url) {
    let obj = await getObjFromApi(url)
    replaceSpan('main-price', obj.price)
}

function replaceSpan(spanName, value) {
    document.getElementById(spanName).innerHTML = value;
}