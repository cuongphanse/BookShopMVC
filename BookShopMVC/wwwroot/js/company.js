var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/Admin/Company/getall',
            "dataSrc": "data" // Chỉ định lấy mảng từ thuộc tính "data" trong JSON
        },
        "columns": [
            { data: 'name', "width": "15%" },      // Khớp với "title" trong JSON
            { data: 'city', "width": "15%" }, 
            { data: 'state', "width": "15%" },     // Khớp với "author"
            { data: 'streetAddress', "width": "15%" }, // Lấy tên danh mục từ object lồng nhau
            { data: 'phoneNumber', "width": "15%" },       // Khớp với "isbn"
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/Admin/Company/Upsert?id=${data}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i> Sửa
                        </a>               
                        <a onClick=Delete('/Admin/Company/Delete/${data}') class="btn btn-danger mx-2">
                            <i class="bi bi-trash-fill"></i> Xóa
                        </a>
                    </div>`
                },
                "width": "20%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Bạn có chắc không??",
        text: "Bạn sẽ không thể hoàn tác điều này!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Đồng ý, xóa đi!",
        cancelButtonText: "Quay lại"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        // HIỆN SWEETALERT2 KHI XÓA THÀNH CÔNG
                        Swal.fire({
                            title: "Đã xóa!",
                            text: data.message,
                            icon: "success"
                        });
                    } else {
                        // Hiện thông báo lỗi nếu có
                        Swal.fire({
                            title: "Lỗi!",
                            text: data.message,
                            icon: "error"
                        });
                    }
                }
            })
        }
    });
}