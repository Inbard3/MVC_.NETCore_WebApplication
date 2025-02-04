﻿//Opening Hours checkboxes and validation
const days = document.querySelectorAll("#openingHours .openingHours-day");
for (let d of days) {
    const checkbox = d.querySelector("[type='checkbox']");
    const from = d.querySelector("[name='from']");
    const to = d.querySelector("[name='to']");
    const day = d.querySelector("[name='days']");

    //Initial states
    if (!checkbox.checked) {
        from.disabled = true;
        to.disabled = true;
    }

    //Treat checkbox change
    checkbox.addEventListener('change', function () {
        from.disabled = to.disabled = !checkbox.checked;
        from.required = to.required = checkbox.checked;

        if (!checkbox.checked)
            d.querySelector(".openingHoursErrors").hidden = true;
    });

    //Treat time range
    from.addEventListener('blur', validateTimeRange);
    to.addEventListener('blur', validateTimeRange);
    function validateTimeRange() {
        if (checkbox.checked && (timeToValue(from.value) >= timeToValue(to.value) || from.value == "" || to.value == "")) {
            d.querySelector(".openingHoursErrors").innerHTML = "Please Enter a Valid Time Range for " + day.value;
            d.querySelector(".openingHoursErrors").hidden = false;
        }
        else {
            d.querySelector(".openingHoursErrors").hidden = true;
        }
    }
}
function timeToValue(str) { return parseInt(str.split(":")[0]) * 60 + parseInt(str.split(":")[1]) }

//Tags multiple selection
$(function () {
    $(".MultipleCheckboxes[name='tags']").multiselect({
        enableClickableOptGroups: true,
        includeSelectAllOption: true,
        nonSelectedText: 'Select'
    });
});

//Chefs multiple selection
$(function () {
    $(".MultipleCheckboxes[name='chefs']").multiselect({
        enableClickableOptGroups: true,
        includeSelectAllOption: false,
        allSelectedText: null,
        nonSelectedText: 'Select',
        enableFiltering: true,
        filterBehavior: 'text',
        includeFilterClearBtn: false,
        buttonWidth: 'auto'
    });
});


//Image file extention validation
const filesInputs = document.querySelectorAll("input[type=file]");
for (let fileInput of filesInputs) {
    fileInput.addEventListener("input", validateFileObjects);
    function validateFileObjects() {
        var allowedExtensions = /\.(jpeg|jpg|png|bmp|gif|jfif)$/i;
        var fileObjects = [...filesInputs[0].files, ...filesInputs[1].files];
        for (let fileObject of fileObjects) {
            var fileName = fileObject.name;
            if (!allowedExtensions.exec(fileName)) {
                document.querySelector(".imageFileErrors").innerHTML = "Please Enter a valid image file for " + fileInput.name;
                document.querySelector(".imageFileErrors").hidden = false;
                fileInput.value = "";
            }
        }
    }
}




