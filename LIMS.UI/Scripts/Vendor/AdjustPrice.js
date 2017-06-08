(function () {
    var lims = window.lims = window.lims || {};
    if (lims.adjustPrice) {
        return;
    }

    var _context, _$container;
    function _init() {
        _$container = $("#adjustPriceFormModal");
        
        if (!_context.orderDetail) {
            _showError("订单不存在！");
            return;
        }

        lims.util.asPrice($("#price", _$container));

        $("#btnConfirm", _$container).click(function () {
            _save();
        });
    }

    function _showError(message) {
        $("#error", _$container).text(message).removeClass("hidden");
    }

    function _save() {
        var price = $("#price", _$container).val();

        price = parseFloat(price || -1);
        if (price <= 0) {
            _showError("请输入调整的价格（大于零）！");
            return;
        }

        if (price === _context.orderDetail.price) {
            _showError("调整前后的价格相同！");
            return;
        }

        $.ajax({
            url: window.baseUrl + "Main/Vendor/SaveAdjustPrice",
            contentType: "application/json",
            type: "post",
            dataType: "json",
            data: JSON.stringify({ id: _context.orderDetail.id, price: price })
        }).done(function (response) {
            if (response.isSuccess) {
                if ($.isFunction(_onClose)) {
                    _onClose();
                }
                $("#btnClose").click();
            }
        });
    }

    function _onClose(callback) {
        if ($.isFunction(callback)) {
            _onClose = callback;
        }
    }

    lims.adjustPrice = {
        init: function (context) {
            _context = context;

            _init();
        },
        onClose: _onClose
    };
})();