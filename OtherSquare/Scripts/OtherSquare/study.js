var updateList = function (listType, listID) {
    $.ajax({
        type: "GET",
        url: "/Study/SCF_List?type=" + listType,
        success: function (data) {
            $('#' + listID).html(data);
            DrawAttention(listID, "#36d500");
        }
    });
};

/**
 * Creates event listeners for the form elements in the Study area
 */
var Study = function () {
    $('.createEndpoint').click(function () {
        var URL = "/Study/" + $(this).data("endpoint");
        $.ajax({
            type: "GET",
            url: URL,
            success: function (data) {
                $('#detailsView').html(data);
            }
        });
    });

    $('.scf-list-item').unbind("click");
    $('.scf-list-item').click(function () {
        var URL = "/Study/" + $(this).data("endpoint") + '?guid=' + $(this).data("guid");
        var id = $(this).attr("id");
        $.ajax({
            type: "GET",
            url: URL,
            success: function (data) {
                $('#detailsView').html(data);
                SetSelected("scf-list-item", id);
                Study();
            },
            error: function (data) {
                alert('ERROR');
            }
        });
    });

    // #region Subject Details

    $('#saveSubject').unbind("click");
    $('#saveSubject').click(function () {
        if ($('#subjectTitle').val().trim() === '') {
            DrawAttention('subjectTitle', 'red');
            return;
        }
        var URL = "/Study/Subject_Update";
        URL += '?guid=' + $('#subjectGuid').val();
        URL += '&title=' + encodeURIComponent($('#subjectTitle').val());
        $.ajax({
            type: "GET",
            url: URL,
            success: function (data) {
                $('detailsView').html(data);
                updateList("Subject", "SCF_Subjects");
            },
            error: function (data) {
                alert('ERROR');
            }
        });
    });

    $('#deleteSubject').unbind("click");
    $('#deleteSubject').click(function () {
        var URL = "/Study/Subject_Delete";
        URL += '?guid=' + $('#subjectGuid').val();
        $.ajax({
            type: "GET",
            url: URL,
            success: function (data) {
                $('#detailsView').html("");
                updateList("Subject", "SCF_Subjects");
            },
            error: function (data) {
                alert('ERROR');
            }
        });
    });

    $('#remove-category').unbind('click');
    $('#remove-category').click(function () {
        var URL = "/Study/Subject_RemoveCategory";
        URL += '?subjectGuid=' + $('#subjectGuid').val();
        URL += '&categoryGuid=' + $('#remove-category').data('category-guid');
        console.log("category-guid " + $('#remove-category').data('category-guid'));
        console.log("URL " + URL);
        $.ajax({
            type: "GET",
            url: URL,
            success: function (data) {
                $('#detailsView').html(data);
                DrawAttention('category-section', "#36d500");
            },
            error: function (data) {
                alert('ERROR');
            }
        });
    });

    $('.category-section-list-item').unbind('click');
    $('.category-section-list-item').click(function () {
        if ($(this).data('category-guid') !== '') {

        }
    });

// #endregion
};