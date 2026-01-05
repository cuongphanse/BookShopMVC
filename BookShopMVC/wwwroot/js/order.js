var dataTable;

$(document).ready(function () {
    var urlParams = new URLSearchParams(window.location.search);
    var status = urlParams.get('status') || "all"; // Mặc định là all nếu không có
    loadDataTable(status);
});
console.log(new URLSearchParams(window.location.search).get('status'));
function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/Admin/Order/getall?status=' + status,
            "dataSrc": "data" // Chỉ định lấy mảng từ thuộc tính "data" trong JSON
        },
        "columns": [
            { data: 'id', "width": "10%" },      // Khớp với "title" trong JSON
            { data: 'name', "width": "15%" },       // Khớp với "isbn"
            { data: 'phoneNumber', "width": "15%" }, 
            { data: 'applicationUser.email', "width": "15%" }, 
            { data: 'orderStatus', "width": "15%" }, 
            { data: 'paymentStatus', "width": "10%" },
            {
                data: 'orderTotal',
                "render": function (data) {
                    return data.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                },
                "width": "10%"
            },      
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/Admin/Order/Detail?orderId=${data}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i>
                        </a>                                     
                    </div>`
                },
                "width": "10%"
            }
        ]
    });
}
