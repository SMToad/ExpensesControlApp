function validateForm() {
    $(".validate-form").validate({
        errorElement: "span",
        rules: {
            ExpenseName: "required",
            Amount: "required",
            Date: "required",
            TimeSpan: "required"
        },
        messages: {
            ExpenseName: "The name of the expense is required",
            Amount: "The amount of the expense is required",
            Date: "The date of the expense is required",
            TimeSpan: "Time span of the expense is required"
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

