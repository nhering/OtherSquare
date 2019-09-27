var debug = function (message) {
    let loggingOn = true;
    if (loggingOn) {
        console.log(Date.now() + ' ' + message);
    }
};

var AddAntiForgeryToken = function (data) {
    debug("main.js AddAntiForgeryToken data: " + data);
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};

$('#login').click(function () {
    debug("main.js #login.click");
    window.location.href = '/Account/Login';
});

$('#logout').click(function () {
    debug("main.js #logout.click");
    $.ajax({
        url: "/Account/LogOff",
        type: "POST",
        success: function () {
            window.location = "/Home/Index";
        }
    });
});

$('#entities').click(function () {
    debug("main.js #accounts.click");
    window.location = "/Entity/Search";
});

// #region Entities

$('#entitySearch').click(function () {
    debug("main.js #accounts.click");
    window.location = "/Entity/Search";
});

$('#entityNew').click(function () {
    debug("main.js #accounts.click");
    window.location = "/Entity/New";
});

$('#inputEntitySearch').change(function () {
    debug("main.js #entitySearch.change " + $(this).val());
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
    let data = form.serialize();
    form.submit(function (event) {
        let index = 0;
        $.each($('.entityProperty'), function () {
            $(this).find('.guid').attr('name', 'Properties[' + index + '].EntityPropertyGuid');
            $(this).find('.key').attr('name', 'Properties[' + index + '].Key');
            $(this).find('.value').attr('name', 'Properties[' + index + '].Value');
            index++;
        });
        event.preventDefault();
        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            data: form.serialize()
        });
    });
});

$('#newEntityProperty').click(function () {
    debug("main.js #newEntityProperty.click");
    let entityProps = $('#entityProperties').html();
    $.ajax({
        type: "GET",
        url: "/Entity/NewProperty",
        success: function (data) {
            $('#entityProperties').html(entityProps + data);
        }
    });
});

// #endregion