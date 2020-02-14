/**
 * Clears existing event listeners (to avoid duplicates) and recreates event listeners for the form elements in the LMS area.
 */
var LMS = function () {
    //$('.area-nav').unbind('click');
    //$('.area-nav').click(function () {
    //    window.location = $(this).data("location");
    //});

    $('#applyDateRange').unbind('click');
    $('#applyDateRange').click(function () { ApplyDateRange(); });
    
    $('#defaultDateRange').unbind('click');
    $('#defaultDateRange').click(function () { DefaultDateRange(); });

    $('.button-new').unbind('click');
    $('.button-new').click(function () {
        let type = $(this).data("type");
        settingsObj["SelectedFlashcard"] = "";
        if (type === "category" || type === "subject") {
            settingsObj["SelectedCategory"] = "";
        }
        if (type === "subject") {
            settingsObj["SelectedSubject"] = "";
        }
        SaveSettings(function () {
            let fla = type.tolowercase() === "flashcard" ? function () { $("#" + type + "Title").focus(); } : "undefined";
            LoadPartialView("GetFlashcardPartial", "flashcards", fla);
            if (type === "category" || type === "subject") {
                let cat = type.tolowercase() === "category" ? function () { $("#" + type + "Title").focus(); } : "undefined";
                LoadPartialView("GetCategoryPartial", "categories", cat);
            }
            if (type === "subject") {
                let sub = type.tolowercase() === "subject" ? function () { $("#" + type + "Title").focus(); } : "undefined";
                LoadPartialView("GetSubjectPartial", "subjects", sub);
            }
        });
    });

    $('.setting').unbind('click');
    $('.setting').click(function () {
        if ($(this).data("behavior") !== "click") return;
        console.log(".setting.click()");
        let name = $(this).attr("name");
        switch (name) {
            case "SettingsAccordionExpanded":
                value = !settingsObj[name];
                $('.setting-section').toggleClass("expanded");
                $('.setting-section').toggleClass("collapsed");
                settingsObj[name] = value;
                SaveSettings();
                break;
            case "IncludeArchive":
                value = $(this).is(":checked");
                $('.unarchive').toggleClass("hide");
                settingsObj[name] = value;
                SaveSettings(function () {
                    window.location = "/Home/Index";
                }, function () {
                    alert("ERROR: The server may not have saved the setting change.");
                });
                break;
            case "SubjectAccordionExpanded":
                value = !settingsObj[name];
                $('.subject-section').toggleClass("expanded");
                $('.subject-section').toggleClass("collapsed");
                settingsObj[name] = value;
                SaveSettings();
                break;
            case "CategoryAccordionExpanded":
                value = !settingsObj[name];
                $('.category-section').toggleClass("expanded");
                $('.category-section').toggleClass("collapsed");
                settingsObj[name] = value;
                SaveSettings();
                break;
            case "FlashcardAccordionExpanded":
                value = !settingsObj[name];
                $('.flashcard-section').toggleClass("expanded");
                $('.flashcard-section').toggleClass("collapsed");
                settingsObj[name] = value;
                SaveSettings();
                break;
            default:
                break;
        }
    });

    $('.setting').unbind('change');
    $('.setting').change(function () {
        if ($(this).data("behavior") !== "change") return;
        console.log(".setting.change()");
        let name = $(this).attr("name");
        let value;
        switch (name) {
            case "BeginDate":
            case "EndDate":
                DrawAttention("applyDateRange", "#fac04b", false);
                break;
            default:
                value = $(this).val();
                settingsObj[name] = value;
                break;
        }
    });

    $('.date-picker').datepicker({
        dateFormat: "mm/dd/yy",
        maxDate: new Date()
    });

    $('.button-save').unbind('click');
    $('.button-save').click(function () {
        let type = $(this).data("type");
        SaveObject(type);
    });

    $('.title-input').unbind('change');
    $('.title-input').change(function () {
        let type = $(this).data("type");
        settingsObj["Selected" + ToProperCase(type)]["Title"] = $(this).val();
    });

    InitializeDateSettings();

    InitializeSectionControls();
};

var InitializeDateSettings = function () {
    console.log("initializeDateSettings");
    let begin = moment.utc($("#BeginDate").val()).format('MM/DD/YYYY');
    let end = moment.utc($("#EndDate").val()).format('MM/DD/YYYY');
    console.log("Begin " + begin);
    $("#BeginDate").val(begin);
    $("#EndDate").val(end);
};

var ApplyDateRange = function () {
    let begin = $('#BeginDate').val();
    let end = $('#EndDate').val();
    if (moment(end) > moment(begin)) {
        settingsObj["BeginDate"] = $('#BeginDate').val();
        settingsObj["EndDate"] = $('#EndDate').val();
        SaveSettings("/LMS", settingsObj);
        window.location = "/Home/Index";
    } else {
        DrawAttention("BeginDate", "red");
        DrawAttention("EndDate", "red");
        $('#BeginDate').attr("title", "The 'From' date must be before the 'To' date");
        $('#EndDate').attr("title", "The 'From' date must be before the 'To' date");
    }
};

var DefaultDateRange = function () {
    let begin = moment(new Date().toLocaleDateString()).subtract(6, 'months');
    $('#BeginDate').val(new Date(begin).toLocaleDateString());
    $('#EndDate').val(new Date().toLocaleDateString());
    ApplyDateRange();
};

var InitializeSectionControls = function () {
    let newBtns = document.querySelectorAll()
    if (settingsObj["SelectedSubject"]["SubjectGuid"] === Guid.empty) {
        console.log("InitializeSectionControls SubjectGuid");
        // Lock the category and flashcard sections
        //  get the .button-new & data-type elements and add the 'disabled' class
    } else if (settingsObj["SelectedCategory"]["CategoryGuid"] === Guid.empty) {
        console.log("InitializeSectionControls CategoryGuid");
        // Lock the flashcard section
    }
};

var LoadPartialView = function (endpoint, sectionId, success, error) {
    console.log("loadPartialView called: " + endpoint);
    let url = redirectUrl + "/" + endpoint;
    data = {
        RedirectURL: "/" + redirectUrl,
        SettingsJSON: JSON.stringify(settingsObj)
    };
    $.ajax({
        url: url,
        type: "GET",
        data: data,
        success: function (response) {
            $('#' + sectionId).html(response);
            if (typeof success !== "undefined") success();
        },
        error: function () {
            console.log("loadPartialView failed for " + sectionId);
            if (typeof error !== "undefined") error();
        }
    });
};

var SaveObject = function (type) {
    console.log("SaveObject called");
    $.ajax({
        url: "api/LMS/Save" + ToProperCase(type),
        type: "POST",
        data: settingsObj["Selected" + ToProperCase(type)],
        success: function (response) {
            console.log("SaveObject success: " + JSON.stringify(response));
            let val = InputValidation.fromJson(response);
            if (val.cssClass === "") {
                settingsObj["Selected" + type] = val.object;
                LoadPartialView("Get" + ToProperCase(type) + "Partial", type.tolowercase());
                //LoadPartialView("GetFlashcardPartial", "flashcards");
                //if (type === "category" || type === "subject") {
                //    LoadPartialView("GetCategoryPartial", "categories");
                //}
                //if (type === "subject") {
                //    LoadPartialView("GetSubjectPartial", "subjects");
                //}
            } else {
                val.showValidation();
            }
        },
        error: function (response) {
            console.log("SaveObject error: " + JSON.stringify(response));
        }
    });
};