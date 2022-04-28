
$('#print').on('click', function () {
    PrintReport();
});

function PrintReport() {
    $.get("/Content/Printing.css", function (cssContent) {
        Print(cssContent);
    });
}

function Print(css) {
    // var height = document.getElementById('printPriview').offsetHeight;
    // var heightmm = Math.floor(height * 0.38);
    //  var res = css.replace(/size: 290mm 210mm;/g, "size: 290mm 210mm;");
    var divToPrint = document.getElementById("report");
    newWin = window.open("");
    newWin.document.write("<html><head> ");
    newWin.document.write("<style>" + css + " </style><title></title></head>");
    newWin.document.write("<body>" + divToPrint.outerHTML + "</body>");
    newWin.print();
    newWin.close();
}

$(".filterdate").attr("autocomplete", "off");
$.getScript("/plugins/datepicker/bootstrap-datepicker.js", function () {
    $('.fromdate').datepicker({
        autoclose: true,
        daysOfWeekDisabled: [0]
    }).on('changeDate', function (ev) {
        $('#From').val(ev.date);
    });

    $('.todate').datepicker({
        autoclose: true,
        daysOfWeekDisabled: [0]
    }).on('changeDate', function (ev) {
        $('#To').val(ev.date);
    });

});

$('#Division').on('change', function () {
    $('#DivisionName').val($('#Division option:selected').text());
});

$('#Location').on('change', function () {
    $('#LocationName').val($('#Location option:selected').text());
});

$('#CboDivisionCode').on('change', function () {
    $('#DivisionCode').val($('#CboDivisionCode option:selected').text());
});
 
$('#CboStatus').on('change', function () {
    $('#StatusView').val($('#CboStatus option:selected').text());
});

$('#view').on('click', function () {
    $('#DivisionName').val($('#Division option:selected').text());
    $('#LocationName').val($('#Location option:selected').text());
    $('#DivisionCode').val($('#CboDivisionCode option:selected').text());
    $('#StatusView').val($('#CboStatus option:selected').text());
});


$('#word').on('click', function () {
    var header = "<html xmlns:o='urn:schemas-microsoft-com:office:office' " +
         "xmlns:w='urn:schemas-microsoft-com:office:word' " +
         "xmlns='http://www.w3.org/TR/REC-html40'>" +
         "<head><meta charset='utf-8'><title>In Out Report</title></head><body>";
    var footer = "</body></html>";
    var sourceHTML = header + document.getElementById("report").innerHTML + footer;

    var source = 'data:application/vnd.ms-word;charset=utf-8,' + encodeURIComponent(sourceHTML);
    var fileDownload = document.createElement("a");
    document.body.appendChild(fileDownload);
    fileDownload.href = source;
    fileDownload.download = 'document.doc';
    fileDownload.click();
    document.body.removeChild(fileDownload);
});