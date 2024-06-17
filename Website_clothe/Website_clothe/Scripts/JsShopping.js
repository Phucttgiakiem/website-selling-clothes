$(document).ready(function () {
    $('body').on('click', '.btnAddtoCard', function (e) {
        e.preventDefault();
        var id = $(this).data('id')
        var quatity = 1
        var color_of_p = $('#code_color').text()
        color_of_p = color_of_p.split(":")[1]



        var size_of_p = $('#code_size').text()
        size_of_p = size_of_p.split(":")[1]

        if (color_of_p.trim() == "") {
            alert("bạn chưa chọn màu");
            return;
        }
        if (size_of_p.trim() == "") {
            alert("bạn chưa chọn size");
            return;
        }
        if ($('.soldout').css('display') === "block") {
            alert('Đã hết sản phẩm với mong muốn bạn chọn vui lòng chọn sản phẩm khác');
            return;
        }
        $.ajax({
            url: '/Shoppingcard/Addtocart',
            type: 'POST',
            data: {
                id: id,
                quantity: quatity,
                colorpd: color_of_p,
                sizepd: size_of_p
            },
            success: function (rs) {
                if (rs.Success) {
                    alert(rs.msg);
                }
            }
        });
    })
    $('body').on('click', '.btnUpdateCard', function (e){
        e.preventDefault();
        var id = $(this).data('id')
        var quatity = $('#Quantity_' + id).val()
        UpdateCard(id, quatity)
    })
    $('body').on('click', '.btnDeleteCard', function (e){
        e.preventDefault();
        var id = $(this).data('id')
        var conf = confirm("Bạn có chắn chắn muốn xóa sản phẩm này ?")
        if (conf) {
            $.ajax({
                url: '/Shoppingcard/DeleteCard',
                type: 'POST',
                data: {
                    id:id
                },
                success: function (rs) {
                    if (rs.Success) {
                        LoadCard();
                    }
                }
            })
        }
    })
    $('body').on('click', '.btnDeleteAll', function(e) {
        e.preventDefault();
        var conf = confirm("Bạn có chắc chắn muốn tất cả các sản phẩm khỏi giỏ hàng ?")
        if (conf) {
            DeleteAll()
        }
    })
})
function DeleteAll() {
    $.ajax({
        url: '/Shoppingcard/DeleteAll',
        type: 'POST',
        success: function (rs) {
            if (rs.Success) {
                LoadCard()
            }
        }
    })
}
function UpdateCard(id,soluong) {
    $.ajax({
        url: '/Shoppingcard/Update',
        type: 'POST',
        data: {
            id: id,
            soluong: soluong
        },
        success: function (rs) {
            if (rs.Success) {
                LoadCard()
            }
            else {
                alert(rs.msg);
            }
        }
     })
}
function LoadCard() {
    $.ajax({
        url: '/Shoppingcard/Partial_Item_Cart',
        type: 'GET',
        success: function (rs) {
            $('#load_data_shopping').html(rs);
        }
    })
}
