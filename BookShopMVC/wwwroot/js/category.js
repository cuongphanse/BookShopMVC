var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/Admin/Category/getall',
            "dataSrc": "data" // Chỉ định lấy mảng từ thuộc tính "data" trong JSON
        },
        "columns": [
            { data: 'displayOrder', "width": "15%", "className": "text-start"  },     
            { data: 'name', "width": "55%" },         
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/Admin/Category/Upsert?id=${data}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i> Sửa
                        </a>               
                        <a onClick=Delete('/Admin/Category/Delete/${data}') class="btn btn-danger mx-2">
                            <i class="bi bi-trash-fill"></i> Xóa
                        </a>
                    </div>`
                },
                "width": "30%"
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