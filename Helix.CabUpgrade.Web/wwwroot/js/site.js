// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function Train() {
    console.log('calling train endpoint');

    jQuery.ajax({
        url: "/Api/Train",
        type: "POST",
        data: "{ 'Empty': true }",//JSON.stringify(patch),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            console.log(data.Message);
            // do something else here
        }
    });
}

function Test() {
    console.log('calling test endpoint');

    jQuery.ajax({
        url: "Api/Test",
        type: "GET",
        success: function () {
            console.log('OK');
            // do something else here
        }
    });
}