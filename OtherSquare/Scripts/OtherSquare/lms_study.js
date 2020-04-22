var LMSStudy = function () {
    redirectUrl = "/LMS/Study";
    SetNavBreadcrumbs(["LMS", "LMS_Study"]);
    SaveSettings();

    $('.item-list-item').each(function () {
        //Set up the ability to count the checked checkboxes to
        //add remove guids from the SelectedCategories list
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
                $("#subjectTitle").text($(this).data("title"));
                $(this).addClass("selected");
            }
            $(this).click(function () {
                if (settingsObj["SelectedSubjectGuid"] === $(this).data("guid")) return;
                settingsObj["SelectedSubjectGuid"] = $(this).data("guid");
                SaveSettings(function () {
                    Loading.begin();
                    window.location = redirectUrl;
                });
            });
        }
    });

    $(':checkbox').click(function () {
        if ($(this).val() === "on") {
            $(this).val("");
        } else {
            $(this).val("on");
        }
        UpdateSettingsInfo();
    });

    UpdateSettingsInfo();
};

$("#startQuiz").click(function () {
    Loading.begin();
    settingsObj["QuizInProgress"] = true;
    let data = AddAntiForgeryToken({
        RedirectURL: redirectUrl,
        SettingsJSON: JSON.stringify(settingsObj)
    });
    $.ajax({
        url: "/api/LMS/StartQuiz",
        type: "POST",
        data: data,
        success: function () {
            window.location = redirectUrl;
        },
        error: function () {
            //Deal with errors
        }
    });
});

$("#stopQuiz").click(function () {
    Loading.begin();
    settingsObj["QuizInProgress"] = false;
    SaveSettings(function () {
        window.location = redirectUrl;
    });
});

var UpdateSettingsInfo = function () {
    settingsObj["SelectedCategoryGuids"] = [];
    questionCount = 0;
    $('.item-list-item').each(function () {
        if ($(this).data("type") === "category") {
            if ($(this).children(":input").val() === "on") {
                settingsObj["SelectedCategoryGuids"].push($(this).data("guid"));
                questionCount += $(this).data("count");
            }
        }
    });
    let categoryCount = settingsObj["SelectedCategoryGuids"].length;
    $("#categoryCount").text(categoryCount);
    $("#questionCount").text(questionCount);
    if (categoryCount === 1) {
        $("#pluralizeCategory").text("y");
    } else {
        $("#pluralizeCategory").text("ies");
    }
    if (questionCount === 1) {
        $("#pluralizeQuestions").text("");
    } else {
        $("#pluralizeQuestions").text("s");
    }
    if (questionCount > 0) {
        $("#startQuiz").removeClass("disabled");
    } else {
        $("#startQuiz").addClass("disabled");
    }
};

$("#showAnswer").click(function () {
    $("#answer").removeClass("hide");
    $("#showAnswer").addClass("hide");
});

$("#correct").click(function () {
    settingsObj["CurrentAnswerCorrect"] = true;
    NextQuestion();
});

$("#incorrect").click(function () {
    settingsObj["CurrentAnswerCorrect"] = false;
    NextQuestion();
});

var NextQuestion = function () {
    Loading.begin();
    let data = AddAntiForgeryToken({
        RedirectURL: redirectUrl,
        SettingsJSON: JSON.stringify(settingsObj)
    });
    $.ajax({
        url: "/api/LMS/NextQuestion",
        type: "POST",
        data: data,
        success: function () {
            window.location = redirectUrl;
        },
        error: function () {
            //Deal with errors
        }
    });
};