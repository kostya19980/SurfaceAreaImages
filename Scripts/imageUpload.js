$(document).ready(function () {
    $("#Download").attr('disabled', true);
    $("#S").attr('disabled', true);
    $("#uploadImage").change(function () {
        var file = this.files;
        if (file && file[0]) {
            readImage(file[0]);
            var ImageFile = $("#uploadImage").get(0).files[0];
            var data = new FormData;
            data.append("ImageFile", ImageFile);
            $.ajax({
                type: "Post",
                url: "/Home/UploadImage",
                data: data,
                contentType: false,
                processData: false,
                success: function () {
                    $("#result").css("display", "none");
                    $("#Download").attr('disabled', false);
                    $("#S").attr('disabled', false);
                    var ImageName = ImageFile.name.split('.')[0];
                    $("#Image").attr("value", ImageFile.name);
                    $('<div>', { class: "ToolBarButton", id: ImageName, style: "cursor:pointer;with:120px" }).appendTo('#List');
                    $('<img>', { src: $("#Image").attr("src"), style: 'height: 100%; width: 100%; object-fit: contain;',value:ImageFile.name}).appendTo('#' + ImageName);
                    $('#' + ImageName).children("img")[0].addEventListener('click', chooseImage);
                    $('<span>', {
   text: ImageName, style: "position:relative;display: block;text-align: center;margin-top:-15px;font-size:8pt;color:black;font-weight:900;background-color:cadetblue;z-index:1"
                    }).appendTo('#' + ImageName);
                }
            })    
        }
    })
    $('#ImportExcel').on("change", function () {
        //$("#form1").submit();
        var excelFile = $("#ImportExcel").get(0).files[0];
        var data = new FormData;
        data.append("excelFile", excelFile);
        $.ajax({
            type: "Post",
            url: "/Home/Import",
            data: data,
            contentType: false,
            processData: false,
            success: function (result) {
                if (result == "Неверный тип файла") {
                    alert(result);
                }
                else {
                    $("#result").css("display", "none");
                    $("#Download").attr('disabled', false);
                    $("#S").attr('disabled', false);
                    $("#Image").attr("src", "data:image/jpeg;base64," + result);
                    var ImageName = (excelFile.name).split('.')[0];
                    $("#Download").attr("value", ImageName);
                    $("#Image").attr("value", ImageName + ".jpeg");
                    $('<div>', { class: "ToolBarButton", width: "120px", id: ImageName, style: "cursor:pointer;" }).appendTo('#List');
                    $('<img>', { src: $("#Image").attr("src"), style: 'height: 100%; width: 100%; object-fit: contain;', value: ImageName+".jpeg"}).appendTo('#' + ImageName);
                    $('#' + ImageName).children("img")[0].addEventListener('click', chooseImage);
                    $('<span>', {
                        text: ImageName, style: "position:relative;display: block;text-align: center;margin-top:-15px;font-size:8pt;color:black;font-weight:900;background-color:cadetblue;z-index:1"
                    }).appendTo('#' + ImageName);
                }
            }
        })    
    });
    function chooseImage(e) {
        $("#Image").attr("value", e.target.getAttribute('value'));
        $("#Image").attr("src", e.target.src);
        $("#result").css("display", "none");
        $("#Download").attr("value", $("#Image").attr("value"));
    }
    $("#S").click(function () {
        var fileName = $("#Image").attr("value");
        $.post("/Home/FindS", { "Imagename": fileName, "Z": $("#Z").val(), "width": $("#W").val(), "height": $("#H").val()},
            function (result) {
                $("#result").css("display","block");
                $('#result').text("Площадь: " + result);
            });
    })
})
function changeDot(e) {
    e.value = e.value.replace(/[.]/g, ",");
}
function Download(e) {
    fileName = e.value;
    window.location.href = "/Home/Download?file=" + fileName;
}
function readImage(file) {
    var reader = new FileReader;
    var image = new Image;
    reader.readAsDataURL(file);
    reader.onload = function (file) {
        image.src = file.target.result;
        image.onload = function () {
            $("#Download").attr("value", $("#uploadImage").get(0).files[0].name);
            $("#Image").attr('src', image.src);
            $("#Image").css({ "width": "100%" });
            $("#Image").css({ "height": "100%" });
        }
    }
}
function dataURLtoFile(dataurl, filename) {
    var arr = dataurl.split(','), mime = arr[0].match(/:(.*?);/)[1],
        bstr = atob(arr[1]), n = bstr.length, u8arr = new Uint8Array(n);
    while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
    }
    return new File([u8arr], filename, { type: mime });
}
