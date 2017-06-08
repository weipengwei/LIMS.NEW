(function () {
    var lims = window.lims = window.lims || {};
    if (lims.orderQueryDispatch) {
        return;
    }

    var _context, _$container, _template, _onClose;
    function _init() {
        _$container = $("#orderQueryDispatchModal");
        _template = _$("#itemTemplate").html();

        if (!_context || !_context.ids || _context.ids.length == 0) {
            return;
        }
        
        _load(_context.ids);

        _$("#btnConfirm").click(function () {
            _save();
        });
    }

    function _$(id) {
        return $(id, _$container);
    }

    function _showErr(message) {
        _$("#error").text(message);
    }

    function _load(ids) {
        $.ajax({
            url: window.baseUrl + "Vendor/OrderQuery/LoadOrderItems",
            contentType: "application/json",
            type: "post",
            dataType: "json",
            data: JSON.stringify({ itemIds: ids })
        }).done(function (res) {
            if (res.isSuccess) {
                _bindData(res.data)
            }
        });
    }

    function _bindData(list) {
        var $orderItems = _$("#orderItems");

        list.forEach(function (item) {
            var $row = $(_template).appendTo($orderItems);

            $row.data("item", item);
            $row.find("p:eq(0)").text(item.productName);
            $row.find("p:eq(1)").text(item.dispatchCount);

            var $input = $row.find("input");
            $input.val(item.waitingCount);

            lims.util.numericBinder($input, {
                min: 0,
                max: item.waitingCount,
                point: 0
            });
        });
    }

    function _save() {
        _$("#error").removeClass("has-error");

        if (!lims.dataHelper.validate(_$("#orderItems"))) {
            _$("#error").text("请填写发放数量！").removeClass("hidden");
            return;
        }

        var list = [];
        $.each(_$("div.row"), function () {
            var $this = $(this), item = $this.data("item");

            var value = $this.find("input").val();
            if (value && value > 0) {
                list.push({
                    productId: item.productId,
                    orderId: item.orderId,
                    orderFormNo: item.formNo,
                    orderDetailId: item.id,
                    dispatchedCount: value
                });
            }
        });

        if (list.length == 0) {
            _$("#error").text("没有发放产生，请检查发放数量是否大于零！").removeClass("hidden");
            return;
        }

        $.ajax({
            url: window.baseUrl + "Vendor/OrderQuery/Save",
            contentType: "application/json",
            type: "post",
            dataType: "json",
            data: JSON.stringify({ forms: list })
        }).done(function (response) {
            if (response.isSuccess) {
                _onClose && _onClose();
                _$("#btnClose").click();
            }
        });
    }

    function onClose(callback) {
        if ($.isFunction(callback)) {
            _onClose = callback;
        }
    }

    lims.orderQueryDispatch = {
        init: function (context) {
            _context = context;
            _init();
        },
        onClose: onClose
    };
})();