function validateForm() {
    $(".validate-form").validate({
        rules: {
            ExpenseName: {
                required: true,
                rangelength: [3, 15]
            },
            Amount: {
                required: true,
                number: true,
                min: 0.01
            },
            Date: {
                required: true,
                dateISO: true
            },
            TimeSpan: "required"
        },
        messages: {
            ExpenseName: {
                required: "The name of the expense is required",
                rangelength: "Name length should be in range [3,15]"
            },
            Amount: {
                required: "The amount of the expense is required",
                number: "Amount should only consist of numbers",
                min: "Amount should be more than zero"
            },
            Date: {
                required: "The date of the expense is required",
                dateISO: "Date format should be: DD.MM.YYYY"
            },
            TimeSpan: "Time span of the expense is required"
        },
        errorPlacement: function (error, element) {
            var name = element.attr('name');
            var errorSelector = '.validation_error_message[for="' + name + '"]';
            var $element = $(errorSelector);
            //if ($element.length) {
            //    $(errorSelector).html(error.html());
            //} else {
            error.addClass("text-danger");
            error.insertAfter($element);
            //}
        }
    });
}
function convertTimeSpanInput(timeSpan) {
    $(document).ready(function () {
        $(`input[value="${timeSpan}"]`).prop("checked", true);
    });
}
function setUpDatePicker() {
    $(document).ready(function () {

        $("#datepicker").datepicker({
            dateFormat: "dd.mm.yy",
            firstDay: 1
        });
    });
}
function submitDatePicker() {
    $("#submit").click(function () {
        let date = $("#datepicker").val().split(".").reverse().join("-");
        $("#datepicker").val(date);
    });
}

