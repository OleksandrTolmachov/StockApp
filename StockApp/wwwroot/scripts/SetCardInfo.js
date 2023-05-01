async function setCardInfo(stockSymbol) {
    let response = await fetch(`http://localhost:5130/Stock/GetStockCard/${stockSymbol}`)
    let data = await response.text()
    document.querySelector(".detailed-card").innerHTML = data
    document.querySelector(".card").style.visibility = 'visible'
}