let settingsObj = {};
let redirectUrl = "";

class InputValidation {
    constructor(cssClass = "", message = "", object = {}) {
        this.cssClass = cssClass;
        this.message = message;
        this.object = object;
    }

    static fromJson(jsonString) {
        let obj = JSON.parse(jsonString);
        return new InputValidation(
            obj.CSSClass,
            obj.Message,
            obj.Object
        );
    }

    showValidation() {
        try {
            logger.debug("InputValidation.showValidation()");
            let val = document.getElementById("inputValidation");
            val.classList.add("validation");
            val.classList.add(this.cssClass);
            val.classList.remove("hide");
            val.innerText = this.message;
            val.addEventListener("click", function () {
                document.getElementById("inputValidation").innerText = "";
                val.classList.remove("validation");
                val.classList.remove(this.cssClass);
                val.classList.add("hide");
            });
        } catch (error) {
            logger.error(error);
        }
    }
}

class Loading {
    static begin() {
        logger.debug("loading started");
    }

    static end() {
        logger.debug("loading ended");
    }
}

$('#logout').click(function () {
    $.ajax({
        url: "/Account/LogOff",
        type: "POST",
        success: function () {
            window.location = "/Home/Index";
        }
    });
});

$('.site-nav').click(function () {
    let loc = $(this).data("location");
    if (typeof loc !== "undefined") window.location = loc;
});

$('.area-nav').click(function () {
    let loc = $(this).data("location");
    if (typeof loc !== "undefined") window.location = loc;
});

/**
 * Add the antiforgerytoken to any ajax data object to send to the server
 * @param {object} data The data object in the ajax call
 * @returns {any} The data object with the token added
 */
var AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};

/**
 * Add the class 'selected' to each item in the array argument. Use at the bottom of each site area page to keep the 'tabs' looking like they are selected.
 * @param {Array} IDs The array of HTML elements to apply the selected class to.
 */
var SetNavBreadcrumbs = function (IDs) {
    logger.debug("SetNavBreadcrumbs called");
    for (let i = 0; i < IDs.length; i++) {
        $("#" + IDs[i]).addClass("selected");
    }
};

/**
 * This is the primary interface for saving the settings of any individual page/area of the site.
 * @param {function} success The callback function to execute if the save is successful.
 * @param {function} error The callback function to execute if the save fails.
 */
var SaveSettings = function (success, error) {
    logger.debug("SaveSettings called");
    let data = AddAntiForgeryToken({
        RedirectURL: "/" + redirectUrl,
        SettingsJSON: JSON.stringify(settingsObj)
    });
    //console.log(JSON.stringify(data));
    $.ajax({
        url: "api/Settings/Save",
        type: "POST",
        data: data,
        success: function (response) {
            logger.debug("Settings save succeeded for " + redirectUrl);
            if (typeof success !== "undefined") success(response);
        },
        error: function (response) {
            logger.error("Settings save failed for " + redirectUrl);
            if (typeof error !== "undefined") error(response);
        }
    });
};

/**
 * Change the color of the border of an element for a moment.
 * @param {string} id The id of the element to affect.
 * @param {string} color The color to change to.
 * @param {bool} toggle If this is set to true, the color will switch back to it's original color after 1000ms.
 */
var DrawAttention = function (id, color, toggle = true) {
    var orig = $('#' + id).attr('style');
    if (typeof orig === 'undefined') orig = '';
    $('#' + id).attr('style', orig + 'border-color:' + color + ';box-shadow: 0 0 2px rgba(0,0,0,.2)');
    if (toggle) {
        setTimeout(function () { $('#' + id).attr('style', orig); }, 1000);
    }
};

//#region Helpers

/**
 * !!..THIS IS NOT A REPLACEMENT FOR A TRUE GUID CLASS...!!
 * This just checks to see if a string representation of a Guid is all zeros
 * or supplies a string representation of an empty Guid
 */
class Guid {
    /**
     * Check if a string representation of a Guid is all zeros. In other words Guid.Empty()
     * @param {any} guid The string to evaluate
     * @returns {boolean} True if the supplied string is an empty string or equals "00000000-0000-0000-0000-000000000000"
     */
    static isEmpty(guid) {
        return guid === "" || guid === "00000000-0000-0000-0000-000000000000";
    }
    /**
     * Get a string representation of an empty Guid. E.G. "00000000-0000-0000-0000-000000000000"
     * @returns {string} "00000000-0000-0000-0000-000000000000"
     */
    static empty() {
        return "00000000-0000-0000-0000-000000000000";
    }
}

/**
 * Change the first character in a string to upper case
 * @param {any} str The string to alter.
 * @return {string} The a copy of the str argument with the first character in upper case.
 */
var ToProperCase = function (str) {
    let first = str.substring(0, 1);
    let rest = str.substring(1);
    first = first.toUpperCase();
    let returnValue = first + rest;
    return returnValue;
};

//#endregion