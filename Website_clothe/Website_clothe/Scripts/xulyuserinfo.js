$(document).ready(function () {
    $.ajax({
        url: '/Access/Partial_InfoUser',
        type: 'GET',
        success: function (rs) {
            $('#resuil_rs').html("");
            $('#resuil_rs').html(rs);
        }
    });
    $('body').on('click', '.btn_app', function () {
        var val1 = $('.time--start').val();
        var val2 = $('.time--end').val();
        var startDate = new Date(val1);
        var endDate = new Date(val2);
        if (val1 == "") {
            alert("Bạn chưa chọn ngày bắt đầu");
            return;
        }
        if (val2 == "") {
            alert("Bạn chưa chọn ngày kết thúc");
            return;
        }
        if (startDate >= endDate) {
            alert("Bạn phải lựa chọn ngày bắt đầu nhỏ hơn ngày kết thúc");
            return;
        }
        $.ajax({
            url: '/Shoppingcard/GetBillwithday',
            type: 'GET',
            data: {
                timestart: val1,
                timeend: val2
            },
            success: function (rs) {
                $('#content-body').html("");
                $('#content-body').html(rs);
            }
        });
        
    })
    $('body').on('click', '.btnseeinfo', function (e) {
        e.preventDefault();
        $.ajax({
            url: '/Access/Partial_InfoUser',
            type: 'GET',
            success: function (rs) {
                $('#resuil_rs').html("");
                $('#resuil_rs').html(rs);
            }
        });
    })
    $('body').on('click', '.btn-Seebill', function (e) {
        e.preventDefault();
        $.ajax({
            url: '/Shoppingcard/GetBillofCustommer',
            type: 'GET',
            success: function (rs) {
                $('#resuil_rs').html("");
                $('#resuil_rs').html(rs);
            }
        });
    })
    $('body').on('click', '.btndetailbill', function (){
        var id = $(this).attr('id');
        $.ajax({
            url: '/Shoppingcard/GetDetailBill',
            type: 'GET',
            data: {
                idbill: id
            },
            success: function (rs) {
                $('#resuil_rs').html("");
                $('#resuil_rs').html(rs);
            }
        })
    })
    $('body').on('click', '.resetbill', function (e){
        e.preventDefault();
        $.ajax({
            url: '/Shoppingcard/GetBillofCustommer',
            type: 'GET',
            success: function (rs) {
                $('#resuil_rs').html("");
                $('#resuil_rs').html(rs);
            }
        });
    })
    $('body').on('click', '.cancel_bill', function () {
        var id = $(this).attr('id');
        var cutstring = id.split('-');
        var iddonhang = cutstring[0];
        var idtrangthai = cutstring[1];

        
        $.ajax({
            url: '/Shoppingcard/CancelBill',
            type: 'POST',
            data: {
                iddonhang: iddonhang,
                idtrangthai: idtrangthai
            },
            success: function (rs) {
                if (rs.Success) {
                    alert("Đã xóa đơn hàng thành công");
                    $.ajax({
                        url: '/Shoppingcard/GetBillofCustommer',
                        type: 'GET',
                        success: function (rs) {
                            $('#resuil_rs').html("");
                            $('#resuil_rs').html(rs);
                        }
                    });
                }
            }
        });
    });
})
