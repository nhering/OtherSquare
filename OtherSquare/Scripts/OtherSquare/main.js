var AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};

// #region Site Navigation

$('#logout').click(function () {
    $.ajax({
        url: "/Account/LogOff",
        type: "POST",
        success: function () {
            window.location = "/Home/Index";
        }
    });
});

$('.site-nav-btn').click(function () {
    window.location = $(this).data("location");
});

$('.obj-detail').click(function () {
    window.location = $(this).data("endpoint") + "Detail?guid=" + $(this).data("guid");
});

/**
 * Add the class 'selected' to the item clicked and
 * remove it from all other elements in the list.
 * @param {string} itemClass The class of the list element the item belongs to.
 * @param {id} id The id of the element to set as selected.
 */
var SetSelected = function (itemClass, id) {
    $('.' + itemClass).each(function () {
        $(this).removeClass('selected');
    });
    $("#" + id).addClass('selected');
};

// #endregion

/**
 * Change the color of the border of an element for a moment.
 * @param {string} id The id of the element to affect.
 * @param {string} color The color to change to.
 */
var DrawAttention = function (id, color) {
    var orig = $('#' + id).attr('style');
    if (typeof orig === 'undefined') orig = '';
    $('#' + id).attr('style', orig + 'border-color:' + color + ';box-shadow: 0 0 15px rgba(0,0,0,.1)');
    setTimeout(function () { $('#' + id).attr('style', orig); }, 1000);
};

// #region Entities

$('#inputEntitySearch').keyup(function () {
    var data = { searchString: $(this).val().trim() };
    $.ajax({
        url: "/Entity/Search",
        type: "POST",
        data: AddAntiForgeryToken(data),
        success: function (data) {
            $('#entitySearchResults').html(data);
        }
    });
});

$(function () {
    var form = $('#entityDetail');
    form.submit(function (event) {
        let index = 0;
        $.each($('.entityProperty'), function () {
            $(this).find('.guid').attr('name', 'Properties[' + index + '].Guid');
            $(this).find('.key').attr('name', 'Properties[' + index + '].Key');
            $(this).find('.value').attr('name', 'Properties[' + index + '].Value');
            index++;
        });
    });
});

$('#newEntityProperty').click(function () {
    //For some reason we have to set the value of the input elements to their UI
    //value before we can capture it in the .html() property of the parent element.
    $.each($('.entityProperty'), function () {
        $(this).find('.guid').attr('value', $(this).find('.guid').val());
        $(this).find('.key').attr('value', $(this).find('.key').val());
        $(this).find('.value').attr('value', $(this).find('.value').val());
    });
    let entityProps = $('#entityProperties').html();
    $.ajax({
        type: "GET",
        url: "/Entity/NewProperty",
        success: function (data) {
            $('#entityProperties').html(entityProps + data);
        }
    });
});

function EntityDetail(elem) {
    window.location = "/Entity/Detail?entityGuid=" + $(elem).find('.guid').val();
}

// #endregion