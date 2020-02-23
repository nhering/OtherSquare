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

    //KEEP
    InitializeDateSettings();

    //#endregion

    InitializeSectionControls();

    //KEEP
    $('.item-list-item').each(function () {
        let parentType = $(this).parent().data("type");
        $(this).data("type", parentType);
        if ($(this).data("type") === "subject") {
            if ($(this).data("guid") === settingsObj["SelectedSubjectGuid"]) {
                $(this).addClass("selected");
            }
            $(this).click(function () {
                settingsObj["SelectedSubjectGuid"] = $(this).data("guid");
                settingsObj["SelectedSubjectTitle"] = $(this).data("title");
                ClearCategoryPartial();
                ClearCategorySettings();
                ClearFlashcardPartial();
                ClearFlashcardSettings();
                Loading.begin();
                SaveSettings(function () {
                    window.location = "/Home/Index";
                });
            });
        } else if ($(this).data("type") === "category") {
            if ($(this).data("guid") === settingsObj["SelectedCategoryGuid"]) {
                $(this).addClass("selected");
            }
            $(this).click(function () {
                settingsObj["SelectedCategoryGuid"] = $(this).data("guid");
                settingsObj["SelectedCategoryTitle"] = $(this).data("title");
                ClearFlashcardPartial();
                ClearFlashcardSettings();
                Loading.begin();
                SaveSettings(function () {
                    window.location = "/Home/Index";
                });
            });
        } else if ($(this).data("type") === "flashcard") {
            if ($(this).data("guid") === settingsObj["SelectedFlashcardGuid"]) {
                $(this).addClass("selected");
            }
            $(this).click(function () {
                settingsObj["SelectedFlashcardGuid"] = $(this).data("guid");
                settingsObj["SelectedFlashcardTitle"] = $(this).data("title");
                Loading.begin();
                SaveSettings(function () {
                    window.location = "/Home/Index";
                });
            });
        }
    });
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


//##btn NewSubject()
$('#newSubject').click(function () {
    //-ClearSubjectSettings()
    ClearSubjectSettings();
    //-ClearCategorySettings()
    ClearCategorySettings();
    //-ClearCategoryPartial()
    ClearCategoryPartial();
    //-LockCategoryPartial()
    LockCategoryPartial();
    //-ClearFlashcardSettings()
    ClearFlashcardSettings();
    //-ClearFlashcardPartial()
    ClearFlashcardPartial();
    //-LockFlashcardPartial()
    LockFlashcardPartial();
    //-SaveSettings()
    SaveSettings();
    $('.item-list-item').each(function () {
        $(this).removeClass("selected");
    });
    //-Focus(SubjectTitleInput)
    $("#subjectTitle").focus();
    $("#subjectTitle").val('');
    console.log(JSON.stringify(settingsObj));
});

//##btn NewCategory()
$('#newCategory').click(function () {
    //-ClearCategorySettings()
    ClearCategorySettings();
    //-ClearFlashcardSettings()
    ClearFlashcardSettings();
    //-ClearFlashcardPartial()
    ClearFlashcardPartial();
    //-LockFlashcardPartial()
    LockFlashcardPartial();
    //-SaveSettings()
    SaveSettings();
    //-Focus(CategoryTitleInput)
    $("#categoryTitle").focus();
    console.log(JSON.stringify(settingsObj));
});

//##btn NewFlashcard()
$("#newFlashcard").click(function () {
    //-ClearFlashcardSettings()
    ClearFlashcardSettings();
    //-set flashcard title input value to ""
    $("#flashcardTitle").val("");
    //-set flaschard question textarea value to ""
    $("#question").val("");
    //-set flaschard answer textarea value to ""
    $("#answer").val("");
    //-SaveSettings()
    SaveSettings();
    //-Focus(FlashcardTitleInput)
    $("#flashcardTitle").focus();
    console.log(JSON.stringify(settingsObj));
});


//## ClearSubjectSettings()
var ClearSubjectSettings = function () {
    //-set settings object selected subject guid to Guid.empty()
    settingsObj["SelectedSubjectGuid"] = Guid.empty();
    //-set settings object selected subject title to ""
    settingsObj["SelectedSubjectTitle"] = "";
};

//## ClearCategorySettings()
var ClearCategorySettings = function () {
    //-set settings object selected category guid to Guid.empty()
    settingsObj["SelectedCategoryGuid"] = Guid.empty();
    //-set settings object selected category title to ""
    settingsObj["SelectedCategoryTitle"] = "";
};

//## ClearFlashcardSettings()
var ClearFlashcardSettings = function () {
    //-set selected flashcard guid to Guid.empty()
    settingsObj["SelectedFlashcardGuid"] = Guid.empty();
    //-set selected flashcard title to ""
    settingsObj["SelectedFlashcardTitle"] = "";
    //-set selected flashcard question to ""
    settingsObj["SelectedFlashcardQuestion"] = "";
    //-set selected flashcard answer to ""
    settingsObj["SelectedFlashcardAnswer"] = "";
};


//## ClearCategoryPartial()
var ClearCategoryPartial = function () {
    //-set inner html of category list to ""
    $("#categoryList").html("");
    //-set category title input value to ""
    $("#categoryTitle").val("");
};

//## ClearFlashcardPartial()
var ClearFlashcardPartial = function () {
    //-set inner html of flashcard list to ""
    $("#flashcardList").html("");
    //-set flashcard title input value to ""
    $("#flashcardTitle").val("");
    //-set flaschard question textarea value to ""
    $("#question").val("");
    //-set flaschard answer textarea value to ""
    $("#answer").val("");
};


//## LockCategoryPartial()
var LockCategoryPartial = function () {
    //-Set new category button to disabled
    $("#newCategory").addClass("disabled");
    //-set category title input to disabled
    $("#categoryTitle").addClass("disabled");
    //-set save category button to diabled
    $("#saveCategory").addClass("disabled");
};

//## LockFlashcardPartial()
var LockFlashcardPartial = function () {
    //-Set new flashcard button to diabled
    $("#newFlashcard").addClass("disabled");
    //-Set flashcard title input to diabled
    $("#flashcardTitle").addClass("disabled");
    //-Set save flashcard button to disabled
    $("#saveFlashcard").addClass("disabled");
    //-Set flashcard question to disabled
    $("#question").addClass("disabled");
    //-Set flashcard answer to disabled
    $("#answer").addClass("disabled");
};


//##btn SaveSubject()
$("#saveSubject").click(function () {
    if ($("#subjectTitle").val().trim() === "") return;
    //-Update the settings object
    //--Set the selected subject title
    settingsObj["SelectedSubjectTitle"] = $("#subjectTitle").val();
    //-Send user settings to api controller: SaveSubject
    let data = AddAntiForgeryToken({
        RedirectURL: "/" + redirectUrl,
        SettingsJSON: JSON.stringify(settingsObj)
    });
    $.ajax({
        url: "api/LMS/SaveSubject",
        type: "POST",
        data: data,
        success: function (response) {
            let val = InputValidation.fromJson(response);
            if (val.cssClass === "" || val.cssClass === "success") {
                window.location = "/Home/Index";
            } else {
                //TODO use the InputValidation class to display the message
                alert(JSON.stringify(val.message));
            }
        },
        error: function (response) {
            //TODO implement error response
            // Should probably be in the form of a new InputValidation object
        }
    });
    //--Controller sends settings object to viewmodel
    //--Viewmodel creates or updates subject as appropriate and saves settings if it succeeds
    //-If everything goes well, reload the page with the newly saved settings
    //--If not, server sends validation object back
});

//##btn SaveCategory()
$("#saveCategory").click(function () {
    if ($("#categoryTitle").val().trim() === "") return;
    //-Update the settings object
    //--Set the selected category title
    settingsObj["SelectedCategoryTitle"] = $("#categoryTitle").val();
    //-Send user settings to api controller: SaveCategory
    let data = AddAntiForgeryToken({
        RedirectURL: "/" + redirectUrl,
        SettingsJSON: JSON.stringify(settingsObj)
    });
    $.ajax({
        url: "api/LMS/SaveCategory",
        type: "POST",
        data: data,
        success: function (response) {
            let val = InputValidation.fromJson(response);
            if (val.cssClass === "" || val.cssClass === "success") {
                window.location = "/Home/Index";
            } else {
                //TODO use the InputValidation class to display the message
                alert(JSON.stringify(val.message));
            }
        },
        error: function (response) {
            //TODO implement error response
            // Should probably be use the InputValidation object
        }
    });
    //--Controller sends settings object to viewmodel
    //--Viewmodel creates or updates category as appropriate and saves settings if it succeeds
    //-If everything goes well, reload the page with the newly saved settings
    //--If not, server sends validation object back
});

//##btn SaveFlashcard()
$("#saveFlashcard").click(function () {
    if ($("#categoryTitle").val().trim() === "") return;
    //-Update the settings object
    //--Set the selected flashcard title, question, and answer
    settingsObj["SelectedFlashcardTitle"] = $("#flashcardTitle").val();
    settingsObj["SelectedFlashcardQuestion"] = $("#question").val();
    settingsObj["SelectedFlashcardAnswer"] = $("#answer").val();
    //-Send user settings to mvc controller: SaveFlashcard
    let data = AddAntiForgeryToken({
        RedirectURL: "/" + redirectUrl,
        SettingsJSON: JSON.stringify(settingsObj)
    });
    $.ajax({
        url: "api/LMS/SaveFlashcard",
        type: "POST",
        data: data,
        success: function (response) {
            let val = InputValidation.fromJson(response);
            if (val.cssClass === "" || val.cssClass === "success") {
                window.location = "/Home/Index";
            } else {
                //TODO use the InputValidation class to display the message
                alert(JSON.stringify(val.message));
            }
        },
        error: function (response) {
            //TODO implement error response
            // Should probably be use the InputValidation object
        }
    });
    //--Controller sends settings object to viewmodel
    //--Viewmodel creates or updates category as appropriate and saves settings if it succeeds
    //-If everything goes well, reload the page with the newly saved settings
    //--If not, server sends validation object back
});


//## InitializeSectionControls()
var InitializeSectionControls = function () {
    //-If the selected subject guid == empty
    if (Guid.isEmpty(settingsObj["SelectedSubjectGuid"])) {
        //--LockCategoryPartial()
        LockCategoryPartial();
        //--LockFlashcardPartial()
        LockFlashcardPartial();
    }
    //-If the selected category guid == empty
    if (Guid.isEmpty(settingsObj["SelectedCategoryGuid"])) {
        //--LockFlashcardPartial()
        LockFlashcardPartial();
    }
};