let allowSelect = true;

var LMSFlashcards = function () {
    redirectUrl = "/LMS/Flashcards";
    SetNavBreadcrumbs(["LMS", "LMS_Flashcards"]);
    SaveSettings();

    //#region Settings

    $('.setting').click(function () {
        if ($(this).data("behavior") !== "click") return;
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
                Loading.begin();
                value = $(this).is(":checked");
                $('.unarchive').toggleClass("hide");
                settingsObj[name] = value;
                SaveSettings(function () {
                    window.location = redirectUrl;
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

    $('.setting').change(function () {
        if ($(this).data("behavior") !== "change") return;
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

    $('#applyDateRange').click(function () {
        ApplyDateRange();
    });

    $('#defaultDateRange').click(function () {
        DefaultDateRange();
    });

    InitializeDateSettings();

    //#endregion

    InitializeSectionControls();
    
    $(':checkbox').mousedown(function () {
        if ($(this).attr("name") === "IncludeArchive") return;
        if ($(this).val() === "on") {
            $(this).val("");
        } else {
            $(this).val("on");
        }
        allowSelect = false;
        let data = {
            guid: $(this).parent().data("guid"),
            modelType: $(this).parent().data("type")
        };
        $.ajax({
            url: "/api/LMS/SelectItem",
            type: "POST",
            data: data,
            success: function () {
                InitializeMultiSelectButtons();
                allowSelect = true;
                return false;
            },
            error: function () {
                return false;
            }
        });
    });

    $('.item-list-item').each(function () {
        //Set up the ability to count the checked checkboxes to
        //enable or disable the "Archive Selected", "Unarchive Selected", 
        //and "Delete Selected" buttons
        if ($(this).children(":input").attr("checked") === "checked") {
            $(this).children(":input").val("on");
        } else {
            $(this).children(":input").val("");
        }

        //Set the data type for each item and wire up
        //the mouseup function
        let parentType = $(this).parent().data("type");
        $(this).data("type", parentType);
        if ($(this).data("type") === "subject") {
            if ($(this).data("guid") === settingsObj["SelectedSubjectGuid"]) {
                $(this).addClass("selected");
            }
            $(this).mouseup(function () {
                if (allowSelect === false){
                    allowSelect = true;
                    return;
                }
                if (settingsObj["SelectedSubjectGuid"] === $(this).data("guid")) return;
                settingsObj["SelectedSubjectGuid"] = $(this).data("guid");
                settingsObj["SelectedSubjectTitle"] = $(this).data("title");
                ClearCategoryPartial();
                ClearCategorySettings();
                ClearFlashcardPartial();
                ClearFlashcardSettings();
                Loading.begin();
                SaveSettings(function () {
                    window.location = redirectUrl;
                });
            });
        } else if ($(this).data("type") === "category") {
            if ($(this).data("guid") === settingsObj["SelectedCategoryGuid"]) {
                $(this).addClass("selected");
            }
            $(this).mouseup(function () {
                if (allowSelect === false) {
                    allowSelect = true;
                    return;
                }
                if (settingsObj["SelectedCategoryGuid"] === $(this).data("guid")) return;
                settingsObj["SelectedCategoryGuid"] = $(this).data("guid");
                settingsObj["SelectedCategoryTitle"] = $(this).data("title");
                ClearFlashcardPartial();
                ClearFlashcardSettings();
                Loading.begin();
                SaveSettings(function () {
                    window.location = redirectUrl;
                });
            });
        } else if ($(this).data("type") === "flashcard") {
            if ($(this).data("guid") === settingsObj["SelectedFlashcardGuid"]) {
                $(this).addClass("selected");
            }
            $(this).mouseup(function () {
                if (allowSelect === false) {
                    allowSelect = true;
                    return;
                }
                if (settingsObj["SelectedFlashcardGuid"] === $(this).data("guid")) return;
                settingsObj["SelectedFlashcardGuid"] = $(this).data("guid");
                settingsObj["SelectedFlashcardTitle"] = $(this).data("title");
                Loading.begin();
                SaveSettings(function () {
                    window.location = redirectUrl;
                });
            });
        }
    });

    InitializeMultiSelectButtons();
};

var InitializeDateSettings = function () {
    let begin = moment.utc($("#BeginDate").val()).format('MM/DD/YYYY');
    let end = moment.utc($("#EndDate").val()).format('MM/DD/YYYY');
    $("#BeginDate").val(begin);
    $("#EndDate").val(end);
};

var ApplyDateRange = function () {
    Loading.begin();
    let begin = $('#BeginDate').val();
    let end = $('#EndDate').val();
    if (moment(end) > moment(begin)) {
        settingsObj["BeginDate"] = $('#BeginDate').val();
        settingsObj["EndDate"] = $('#EndDate').val();
        SaveSettings();
        window.location = redirectUrl;
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

$('#newSubject').click(function () {
    ClearSubjectSettings();
    ClearCategorySettings();
    ClearCategoryPartial();
    LockCategoryPartial();
    ClearFlashcardSettings();
    ClearFlashcardPartial();
    LockFlashcardPartial();
    SaveSettings();
    $('.item-list-item').each(function () {
        $(this).removeClass("selected");
    });
    $("#subjectTitle").focus();
    $("#subjectTitle").val('');
});

$('#newCategory').click(function () {
    ClearCategorySettings();
    ClearFlashcardSettings();
    ClearFlashcardPartial();
    LockFlashcardPartial();
    SaveSettings();
    $("#categoryTitle").focus();
});

$("#newFlashcard").click(function () {
    ClearFlashcardSettings();
    $("#flashcardTitle").val("");
    $("#question").val("");
    $("#answer").val("");
    SaveSettings();
    $("#flashcardTitle").focus();
});

var ClearSubjectSettings = function () {
    settingsObj["SelectedSubjectGuid"] = Guid.empty();
    settingsObj["SelectedSubjectTitle"] = "";
};

var ClearCategorySettings = function () {
    settingsObj["SelectedCategoryGuid"] = Guid.empty();
    settingsObj["SelectedCategoryTitle"] = "";
};

var ClearFlashcardSettings = function () {
    settingsObj["SelectedFlashcardGuid"] = Guid.empty();
    settingsObj["SelectedFlashcardTitle"] = "";
    settingsObj["SelectedFlashcardQuestion"] = "";
    settingsObj["SelectedFlashcardAnswer"] = "";
};

var ClearCategoryPartial = function () {
    $("#categoryList").html("");
    $("#categoryTitle").val("");
};

var ClearFlashcardPartial = function () {
    $("#flashcardList").html("");
    $("#flashcardTitle").val("");
    $("#question").val("");
    $("#answer").val("");
};

var LockCategoryPartial = function () {
    $("#newCategory").addClass("disabled");
    $("#categoryTitle").addClass("disabled");
    $("#saveCategory").addClass("disabled");
};

var LockFlashcardPartial = function () {
    $("#newFlashcard").addClass("disabled");
    $("#flashcardTitle").addClass("disabled");
    $("#saveFlashcard").addClass("disabled");
    $("#question").addClass("disabled");
    $("#answer").addClass("disabled");
};

$("#saveSubject").click(function () {
    if ($("#subjectTitle").val().trim() === "") return;
    settingsObj["SelectedSubjectTitle"] = $("#subjectTitle").val();
    let data = AddAntiForgeryToken({
        RedirectURL: redirectUrl,
        SettingsJSON: JSON.stringify(settingsObj)
    });
    $.ajax({
        url: "/api/LMS/SaveSubject",
        type: "POST",
        data: data,
        success: function (response) {
            let val = InputValidation.fromJson(response);
            if (val.cssClass === "" || val.cssClass === "success") {
                window.location = redirectUrl;
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
});

$("#saveCategory").click(function () {
    if ($("#categoryTitle").val().trim() === "") return;
    settingsObj["SelectedCategoryTitle"] = $("#categoryTitle").val();
    let data = AddAntiForgeryToken({
        RedirectURL: redirectUrl,
        SettingsJSON: JSON.stringify(settingsObj)
    });
    $.ajax({
        url: "/api/LMS/SaveCategory",
        type: "POST",
        data: data,
        success: function (response) {
            let val = InputValidation.fromJson(response);
            if (val.cssClass === "" || val.cssClass === "success") {
                window.location = redirectUrl;
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
});

$("#saveFlashcard").click(function () {
    if ($("#categoryTitle").val().trim() === "") return;
    settingsObj["SelectedFlashcardTitle"] = $("#flashcardTitle").val();
    settingsObj["SelectedFlashcardQuestion"] = $("#question").val();
    settingsObj["SelectedFlashcardAnswer"] = $("#answer").val();
    let data = AddAntiForgeryToken({
        RedirectURL: redirectUrl,
        SettingsJSON: JSON.stringify(settingsObj)
    });
    $.ajax({
        url: "/api/LMS/SaveFlashcard",
        type: "POST",
        data: data,
        success: function (response) {
            let val = InputValidation.fromJson(response);
            if (val.cssClass === "" || val.cssClass === "success") {
                window.location = redirectUrl;
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
});

var InitializeSectionControls = function () {
    if (Guid.isEmpty(settingsObj["SelectedSubjectGuid"])) {
        LockCategoryPartial();
        LockFlashcardPartial();
    }
    if (Guid.isEmpty(settingsObj["SelectedCategoryGuid"])) {
        LockFlashcardPartial();
    }
};

$(".archive").click(function () {
    let type = $(this).data("type");
    let guids = GetSelectedGuids(type);
    Archive_Unarchive(type, guids, "Archive");
});

$(".unarchive").click(function () {
    let type = $(this).data("type");
    let guids = GetSelectedGuids(type);
    Archive_Unarchive(type, guids, "UnArchive");
});

var GetSelectedGuids = function (type) {
    let selectedItems = [];
    $(".item-list-item").each(function () {
        if ($(this).data("type") === type) {
            if ($(this).children(":input").val() === "on") {
                selectedItems.push($(this).data("guid"));
            }
        }
    });
    return selectedItems;
};

var Archive_Unarchive = function (type, guids, action) {
    Loading.begin();
    $.ajax({
        url: "/api/LMS/" + action + "Items",
        type: "POST",
        data: {
            modelType: type,
            guids: guids
        },
        success: function () {
            window.location = "/Home/Index";
        },
        error: function () {
            //do something?
        }
    });
};

$("#deleteFlashcards").click(function () {
    Loading.begin();
    let type = "flashcard";
    let guids = GetSelectedGuids(type);
    $.ajax({
        url: "/api/LMS/RemoveFlashcards",
        type: "POST",
        data: {
            modelType: type,
            guids: guids
        },
        success: function () {
            window.location = "/Home/Index";
        },
        error: function () {
            //do something?
        }
    });
});

var InitializeMultiSelectButtons = function () {
    let selectedSubjects = 0;
    let selectedCategories = 0;
    let selectedFlashcards = 0;
    $(".item-list-item").each(function () {
        switch ($(this).data("type")) {
            case "subject":
                if ($(this).children(":input").val() === "on") {
                    selectedSubjects++;
                }
                break;
            case "category":
                if ($(this).children(":input").val() === "on") {
                    selectedCategories++;
                }
                break;
            case "flashcard":
                if ($(this).children(":input").val() === "on") {
                    selectedFlashcards++;
                }
                break;
        }
    });
    if (selectedSubjects === 0) {
        $("#archiveSubject").addClass("disabled");
        $("#unarchiveSubject").addClass("disabled");
    } else {
        $("#archiveSubject").removeClass("disabled");
        $("#unarchiveSubject").removeClass("disabled");
    }
    if (selectedCategories === 0) {
        $("#archiveCategory").addClass("disabled");
        $("#unarchiveCategory").addClass("disabled");
    } else {
        $("#archiveCategory").removeClass("disabled");
        $("#unarchiveCategory").removeClass("disabled");
    }
    if (selectedFlashcards === 0) {
        $("#deleteFlashcards").addClass("disabled");
    } else {
        $("#deleteFlashcards").removeClass("disabled");
    }
};