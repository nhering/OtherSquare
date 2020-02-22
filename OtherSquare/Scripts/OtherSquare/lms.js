//REMOVE
class Category {
    constructor(subjectGuid) {
        this.CategoryGuid = Guid.empty;
        this.Title = "";
        this.IsArchived = false;
        this.IsSelected = false;
        this.SubjectGuid = subjectGuid;
    }
}

//CONSIDER MOVING CONTENTS OUT AND GETTING RID OF THIS WRAPPER FUNCTION
var LMS = function () {
    //#region Settings


    //KEEP
    $('.setting').unbind('click');
    $('.setting').click(function () {
        if ($(this).data("behavior") !== "click") return;
        logger.debug(".setting.click()");
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

    //KEEP
    $('.setting').unbind('change');
    $('.setting').change(function () {
        if ($(this).data("behavior") !== "change") return;
        logger.debug(".setting.change()");
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

    //KEEP
    $('.date-picker').datepicker({
        dateFormat: "mm/dd/yy",
        maxDate: new Date()
    });

    //KEEP
    $('#applyDateRange').unbind('click');
    $('#applyDateRange').click(function () {
        ApplyDateRange();
    });

    //KEEP
    $('#defaultDateRange').unbind('click');
    $('#defaultDateRange').click(function () {
        DefaultDateRange();
    });

    InitializeDateSettings();

    //#endregion

    //REPLACE
    $('.button-new').unbind('click');
    $('.button-new').click(function () {
        logger.debug(".button-new " + JSON.stringify(settingsObj));
        let type = $(this).data("type");
        settingsObj["SelectedFlashcard"] = "";
        if (type === "category") {
            alert(new Category(settingsObj["SelectedSubject"]["SubjectGuid"]));
            settingsObj["SelectedCategory"] = new Category(settingsObj["SelectedSubject"]["SubjectGuid"]);
        }
        if (type === "subject") {
            settingsObj["SelectedCategory"] = "";
            settingsObj["SelectedSubject"] = "";
        }
        SaveSettings(function () {
            let focus = function () { $("#" + type + "Title").focus(); };
            let fla = type.toLowerCase() === "flashcard" ? focus : "undefined";
            LoadPartialView("GetFlashcardPartial", "flashcards", fla);
            if (type === "category" || type === "subject") {
                let cat = type.toLowerCase() === "category" ? focus : "undefined";
                LoadPartialView("GetCategoryPartial", "categories", cat);
            }
            if (type === "subject") {
                let sub = type.toLowerCase() === "subject" ? focus : "undefined";
                LoadPartialView("GetSubjectPartial", "subjects", sub);
            }
        });
    });

    //REPLACE
    $('.button-save').unbind('click');
    $('.button-save').click(function () {
        let type = $(this).data("type");
        SaveObject(type);
    });

    //REPLACE
    $('.title-input').unbind('change');
    $('.title-input').change(function () {
        let type = $(this).data("type");
        settingsObj["Selected" + ToProperCase(type)]["Title"] = $(this).val();
    });

    //KEEP
    $('.item-list-item').each(function () {
        let parentType = $(this).parent().data("type");
        $(this).data("type", parentType);
        $(this).removeClass("selected");
    });

    //REPLACE
    $('.item-list-item').unbind("click");
    $('.item-list-item').click(function () {
        let type = $(this).data("type");
        let obj = $(this).data("object");
        SelectObject(type, obj);
    });

    //REPLACE
    InitializeSectionControls();
};

//KEEP
var InitializeDateSettings = function () {
    logger.debug("initializeDateSettings");
    let begin = moment.utc($("#BeginDate").val()).format('MM/DD/YYYY');
    let end = moment.utc($("#EndDate").val()).format('MM/DD/YYYY');
    logger.debug("Begin " + begin);
    $("#BeginDate").val(begin);
    $("#EndDate").val(end);
};

//KEEP
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

//KEEP
var DefaultDateRange = function () {
    let begin = moment(new Date().toLocaleDateString()).subtract(6, 'months');
    $('#BeginDate').val(new Date(begin).toLocaleDateString());
    $('#EndDate').val(new Date().toLocaleDateString());
    ApplyDateRange();
};

//REPLACE
var InitializeSectionControls = function () {
    if (settingsObj["SelectedSubject"]["SubjectGuid"] === Guid.empty) {
        logger.debug("InitializeSectionControls SubjectGuid");
        $(".category").each(function () {
            $(this).addClass("disabled");
        });
        $(".flashcard").each(function () {
            $(this).addClass("disabled");
        });
    } else if (settingsObj["SelectedCategory"]["CategoryGuid"] === Guid.empty) {
        logger.debug("InitializeSectionControls CategoryGuid");
        $(".flashcard").each(function () {
            $(this).addClass("disabled");
        });
    }
};

//REPLACE
var LoadPartialView = function (endpoint, sectionId, success, error) {
    logger.debug("loadPartialView called: " + endpoint);
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
            //if (typeof success !== "undefined") success();
            //if (typeof success !== "undefined") alert(success);
            if (success !== "undefined") { success(); }
        },
        error: function () {
            logger.error("loadPartialView failed for " + sectionId);
            if (typeof error !== "undefined") error();
        }
    });
};

//REPLACE
var SaveObject = function (type) {
    logger.debug("SaveObject called\ntype: " + type + "\n" + JSON.stringify(settingsObj));
    $.ajax({
        url: "api/LMS/Save" + ToProperCase(type),
        type: "POST",
        data: settingsObj["Selected" + ToProperCase(type)],
        success: function (response) {
            logger.debug("SaveObject success: " + JSON.stringify(response));
            let val = InputValidation.fromJson(response);
            if (val.cssClass === "") {
                settingsObj["Selected" + type] = val.object;
                LoadPartialView("Get" + ToProperCase(type) + "Partial", type.tolowercase());
            } else {
                val.showValidation();
            }
        },
        error: function (response) {
            logger.error("SaveObject error: " + JSON.stringify(response));
        }
    });
};

//REPLACE
var SelectSubject = function () {

};

//REPLACE
var SelectObject = function (type, obj) {
    Loading.begin();
    settingsObj["Selected" + ToProperCase(type)] = obj;
    SaveSettings(function () {
        let fla = type.toLowerCase() === "flashcard" ? Loading.end() : "undefined";
        LoadPartialView("GetFlashcardPartial", "flashcards", fla);
        if (type === "category" || type === "subject") {
            let cat = type.toLowerCase() === "category" ? Loading.end() : "undefined";
            LoadPartialView("GetCategoryPartial", "categories", cat);
        }
        if (type === "subject") {
            let sub = type.toLowerCase() === "subject" ? Loading.end() : "undefined";
            LoadPartialView("GetSubjectPartial", "subjects", sub);
        }
    });
};




//##btn NewSubject()
//-ClearSubjectSettings()
//-ClearCategorySettings()
//-ClearCategoryPartial()
//-LockCategoryPartial()
//-ClearFlashcardSettings()
//-ClearFlashcardPartial()
//-LockFlashcardPartial()
//-SaveSettings()
//-Focus(SubjectTitleInput)

//##btn NewCategory()
//-ClearCategorySettings()
//-ClearFlashcardSettings()
//-ClearFlashcardPartial()
//-LockFlashcardPartial()
//-SaveSettings()
//-Focus(CategoryTitleInput)

//##btn NewFlashcard()
//-ClearFlashcardPartial()
//-SaveSettings()
//-Focus(FlashcardTitleInput)


//## ClearSubjectSettings()
//-set settings object selected subject guid to Guid.empty()
//-set settings object selected subject title to ""

//## ClearCategorySettings()
//-set settings object selected category guid to Guid.empty()
//-set settings object selected category title to ""

//## ClearCategoryPartial()
//-set inner html of category list to ""
//-set category title input value to ""

//## ClearFlashcardSettings()
//-set selected flashcard guid to Guid.empty()
//-set selected flashcard title to ""
//-set selected flashcard question to ""
//-set selected flashcard answer to ""

//## ClearFlashcardPartial()
//-set settings object selected flashcard guid to Guid.empty()
//-set settings object selected flashcard title to ""
//-set settings object selected flashcard question to ""
//-set settings object selected flashcard answer to ""


//## LockCategoryPartial()
//-Set new category button to disabled
//-set category title input to disabled
//-set save category button to diabled

//## LockFlashcardPartial()
//-Set new flashcard button to diabled
//-Set flashcard title input to diabled
//-Set save flashcard button to disabled
//-Set flashcard question to disabled
//-Set flashcard answer to disabled


//## UnlockCategoryPartial()
//-Set new category button to enabled
//-set category title input to enabled
//-set save category button to enabled

//## UnlockFlashcardPartial()
//-Set new flashcard button to enabled
//-Set flashcard title input to enabled
//-Set save flashcard button to enabled
//-Set flashcard question to enabled
//-Set flashcard answer to enabled


//##btn SaveSubject()
//-Update the settings object
//--Set the selected subject guid and title
//-Send user settings to mvc controller: SaveSubject
//--Controller sends settings object to viewmodel
//--Viewmodel creates or updates subject as appropriate and saves settings if it succeeds
//-If everything goes well, reload the page with the newly saved settings
//--If not, server sends validation object back

//##btn SaveCategory()
//-Update the settings object
//--Set the selected category guid and title
//-Send user settings to mvc controller: SaveCategory
//--Controller sends settings object to viewmodel
//--Viewmodel creates or updates category as appropriate and saves settings if it succeeds
//-If everything goes well, reload the page with the newly saved settings
//--If not, server sends validation object back

//##btn SaveFlashcard()
//-Update the settings object
//--Set the selected flashcard guid, title, question, and answer
//-Send user settings to mvc controller: SaveFlashcard
//--Controller sends settings object to viewmodel
//--Viewmodel creates or updates category as appropriate and saves settings if it succeeds
//-If everything goes well, reload the page with the newly saved settings
//--If not, server sends validation object back


//## InitializeSectionControls()
//-If the selected subject guid == empty
//--LockCategoryPartial()
//--LockFlashcardPartial()
//-Elseif the selected category guid == empty
//--LockFlashcardPartial()


//## SelectSubjectItem()
//-Update the settings object
//-SaveSettings()
//-Reload page

//## SelectCategoryItem()
//-Update the settings object
//-SaveSettings()
//-Reload page

//## SelectFlashcardItem()
//-Update the settings object
//-SaveSettings()
//-Reload page
