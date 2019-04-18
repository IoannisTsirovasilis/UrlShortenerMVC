function changeFilePlaceholder() {
    var files = document.getElementById('File').files;
    if (files.length === 1) {
        var file = files[0];
        $("#BrowseFileButton").text(file.name);
    } else {
        $("#BrowseFileButton").text("Choose file");
    }
}