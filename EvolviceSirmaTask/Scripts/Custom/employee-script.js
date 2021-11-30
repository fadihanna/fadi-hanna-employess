$(document).ready(function () {
    $("#EmployeesFile").on("change", function (evt) {
        evt.preventDefault();
        var formData = new FormData($('#EmployeesFileForm').get(0));
        $.ajax({
            processData: false,
            contentType: false,
            type: 'POST',
            async: false,
            data: formData,
            url: "Home/Calculate/",
            success: function (response) {
                $("#EmployeesTable").DataTable().destroy();
                $("#EmployeesTable").DataTable({
                    "data": jQuery.parseJSON(response.data),
                    "pageLength": 5,
                    "paging": true,
                    "order": [[3, "desc"]],
                    "columns": [
                        { "data": "EmployeeID1", "name": "Employee ID #1", "autoWidth": true },
                        { "data": "EmployeeID2", "name": "Employee ID #2", "autoWidth": true },
                        { "data": "ProjectID", "name": "Project ID", "autoWidth": true },
                        { "data": "Days", "name": "Days worked", "autoWidth": true }
                    ],
                    initComplete: function () {                        
                        var row = $('#EmployeesTable_wrapper tr:first-child')[1];
                        $(row).addClass('first-row')
                    }
                });
            },
            error: function () {
                alert('This file is not valid');
            }
        });
    });
});