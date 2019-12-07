$('.rand-btn').click(function () {
    let rand = new Random();
    rand.init;
    rand.timer = setInterval(function () { rand.randomStyle(); }, 10);
});

class Random {
    _gridGap = 1;
    _colWidth = 0;
    _numOfCol = 0;
    _gridWidth = 0;
    _numOfRow = 0;
    _gridHeight = 0;
    _cellCount = 0;
    timer;

    get init() {
        console.clear();

        this._colWidth = this.randomNumberBetween(15, 25);
        console.log("this._colWidth " + this._colWidth);

        this._numOfCol = this.parentGridNumberOfCols();
        console.log("this._numOfCol " + this._numOfCol);

        this._gridWidth = (this._colWidth + this._gridGap) * this._numOfCol;
        console.log("this._gridWidth " + this._gridWidth);

        this._numOfRow = this.parentGridNumberOfRows();
        console.log("this._numOfRow " + this._numOfRow);

        this._gridHeight = (this._colWidth + this._gridGap) * this._numOfRow;
        console.log("this._gridHeight " + this._gridHeight);

        this._cellCount = this._numOfCol * this._numOfRow;
        console.log("this._cellCount " + this._cellCount);

        this.clearAll();
        $('#random').append(this.parentElement());
        this.addChildren();
    }

    addChildren() {
        for (let i = 0; i < this._cellCount; i++) {
            let e = this.cellElement();
            $(e).attr("id",i);
            $("#rand-parent").append(e);
        }
    }

    randomStyle() {
        let id = this.randomNumberBetween(0, this._cellCount);
        //console.log("randomStyle " + id);
        $("#" + id).attr("style", this.cellStyle());
    }

    //#region Parent Element

    parentElement() {
        let e = this.newdiv();
        $(e).attr("style", this.parentStyle());
        $(e).attr("id", "rand-parent");
        return e;
    }

    parentStyle() {
        let style = "background-color:#000;";
        style += this.parentGrid();
        style += this.parentPosition();
        style += this.parentPadding();
        return style;
    }

    parentGrid() {
        let display = "display:grid;";
        let columns = "grid-template-columns:repeat(" + this._numOfCol + "," + this._colWidth + "px);";
        let rows = "grid-template-rows:repeat(" + this._numOfRow + "," + this._colWidth + "px);";
        let gap = "grid-gap:" + this._gridGap + "px;";
        return display + columns + rows + gap;
    }

    parentGridNumberOfCols() {
        let adjustedWidth = window.innerWidth;
        return Math.floor(adjustedWidth / (this._colWidth + this._gridGap)) - 2;
    }

    parentGridNumberOfRows() {
        let adjustedHeight = window.innerHeight - window.innerHeight * .05;
        return Math.floor(adjustedHeight / (this._colWidth + this._gridGap)) - 2;
    }

    parentPosition() {
        return "position:absolute;top:" + this.parentPositionTop() + "px;right:0;bottom:0;left:0;";
    }

    parentPositionTop() {
        let pad = $('.site-nav').css("padding");
        return Number($('.site-nav').height() + Number(pad.substring(0, pad.length - 2)) * 2);
    }

    parentPadding() {
        let pl = Math.round((window.innerWidth - this._gridWidth) / 2);
        let pt = Math.round((window.innerHeight - this.parentPositionTop() - this._gridHeight) / 2);
        return "padding-left:" + (pl + 1) + "px;padding-top:" + pt + "px;";
    }

    //#endregion

    //#region Cell Element

    cellElement() {
        let e = this.newdiv();
        $(e).addClass("rand-elem");
        return $(e).attr("style", this.cellStyle());
    }

    cellStyle() {
        let style = "";
        style += this.styleColor();
        style += this.styleWidth();
        style += this.styleHeight();
        style += this.styleBorderRadius();
        style += this.styleRotate();
        return style;
    }

    styleColor() {
        let r = Math.floor(Math.random() * 256);
        let g = Math.floor(Math.random() * 256);
        let b = Math.floor(Math.random() * 256);
        let a = Number.parseInt(Math.random() * 100,10);
        //console.log(a);
        return "background-color:rgba(" + r + "," + g + "," + b + ",." + a + ");";
        //return "background-color:rgb(" + r + "," + g + "," + b + ");";
    }

    styleRotate() {
        let deg = this.randomNumberBetween(0, 180);
        return "transform:rotate(" + deg + "deg);";
    }

    styleBorderRadius() {
        return "border-radius:" + this.randomNumberBetween(1, 100) + "%;";
    }

    styleWidth() {
        return "width:" + this.randomNumberBetween(1, this._colWidth * 2) + "px;";
    }

    styleHeight() {
        return "height:" + this.randomNumberBetween(1, this._colWidth * 2) + "px;";
    }

    //#endregion

    clearAll() {
        $('#random').html('');
    }

    newdiv() {
        return document.createElement("div");
    }

    randomNumberBetween(a, b) {
        let top = Math.max(a, b) - Math.min(a, b);
        let r = Math.round(Math.random() * top) + Math.min(a, b);
        //console.log("randomNumberBetween(" + a + "," + b + ") = " + r);
        return r;
    }
}