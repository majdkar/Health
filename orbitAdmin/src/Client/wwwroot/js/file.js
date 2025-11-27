window.Download = (options) => {
    var fileUrl = "data:" + options.mimeType + ";base64," + options.byteArray;
    fetch(fileUrl)
        .then(response => response.blob())
        .then(blob => {
            var link = window.document.createElement("a");
            link.href = window.URL.createObjectURL(blob, { type: options.mimeType });
            link.download = options.fileName;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        });
}


function myPrint(host) {
    var divContents = document.getElementById("myPrint").innerHTML;
    var a = window.open('', '', 'height:100%', 'width:1200px');
    a.document.write('<html>');
    a.document.write('<head>');
    a.document.write('<title>Al Sharq International School</title>');
    a.document.write('<link href="https://fonts.googleapis.com/css2?family=Montserrat:wght@400;500;600;700&display=swap" rel="stylesheet">');
    a.document.write('<link href="https://fonts.googleapis.com/css?family=Tajawal" rel="stylesheet" />');
    a.document.write('<link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />');
    a.document.write('<link href="' + host + '_content/MudBlazor/MudBlazor.min.css?v=5.0.5" rel="stylesheet" />');
    a.document.write('<link href="' + host + 'css/loader.css" rel="stylesheet" />');
    a.document.write('</head>');
    a.document.write('<body> <br>');
    a.document.write(divContents);
    a.document.write('</body></html>');
    a.document.close();
    setTimeout(() => { a.print() }, 1000);
}

function PreventEnterKey() {
    $(`#pEK`).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });
}

function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

async function GetElementHtmlText(elementID) {
    await setTimeout(() => { }, 1);
    console.log(elementID);
    var element = document.getElementById(elementID);
    console.log(element);
    var result = element.innerHTML;
    console.log(result);
    return result;
}

//var doc = new jsPDF();
//var specialElementHandlers = {
//    '#editor': function (element, renderer) {
//        return true;
//    }
//}


function generatePDF() {
    let doc = new jsPDF('p', 'pt', 'a4');
    doc.setFontSize(18);

    doc.addHTML(document.getElementById("myPrint"), function () {
        doc.save('testpoc.pdf');
    });

}