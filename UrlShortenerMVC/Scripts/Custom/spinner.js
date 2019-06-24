function disableSubmitButton() {
    $(document).ready(function () {
        if ($("#submit") != null && $("#form").valid()) {
            // When the button is pressed, disable it, hide its text and show spinner 
            $("#submit").prop("disabled", true);
            $("#spinner").prop("hidden", false);
            $("#spinner-label").prop("hidden", true);
        } else {
            $("#submit").prop("disabled", false);
            $("#spinner").prop("hidden", true);
            $("#spinner-label").prop("hidden", false);
        }
    });
}
