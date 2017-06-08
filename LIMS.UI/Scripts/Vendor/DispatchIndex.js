(function () {
    var lims = window.lims = window.lims || {};
    if (lims.dispatchIndex) {
        return;
    }

    var _$container, _template, _pageIndex, _condition;
    function _init() {
        _$container = $("#dispatchIndex");
        _template = $("#formTemplate", _$container).html();

        $("#btnQuery", _$container).click(function () {
            _pageIndex = 0;
            _condition = {};
            
            var formNo = $("#formNo", _$container).val();
            if (formNo) {
                _condition.formNo = parseInt(formNo);
            }

            _condition.status = $("#confirmStatus", _$container).val();

            _query();
        });

        lims.pager.init($("#pageList", _$container), function (newPageIndex) {
            _pageIndex = newPageIndex;
            _query();
        });

        $("#formList", _$container).on("click", "a.create", function () {
            var $tr = $(this).parents("tr"), id = $tr.data("id");
            if (id) {
                lims.window.pop("Vendor/Dispatch/ItemCreator?Id=" + id, function () {
                    if (lims.dispatchItemCreator) {
                        lims.dispatchItemCreator.refresh(function () {
                            _query();
                        });
                    }
                });
            }
        }).on("click", "a.items", function () {
            var $tr = $(this).parents("tr"), id = $tr.data("id");
            if (id) {
                lims.window.pop("Vendor/Dispatch/ItemsView?Id=" + id);
            }
        }).on("click", "a.cancel", function () {
            var $tr = $(this).parents("tr"), id = $tr.data("id");
            if (id) {
                cancel(id, function () {
                    _query();
                });
            }
        });

        $("#btnScan", _$container).click(function () {
            lims.window.pop("Vendor/Dispatch/Confirm", function () {
                if (lims.dispatchConfirm) {
                    lims.dispatchConfirm.refresh(function () {
                        _query();
                    });
                }
            });
        });

        $("#btnQuery", _$container).click();
    }

    function _query() {
        var data = {
            condition: _condition,
            pager: { pageIndex: _pageIndex }
        };

        $.ajax({
            url: window.baseUrl + "Vendor/Dispatch/Query",
            contentType: "application/json",
            type: "post",
            dataType: "json",
            data: JSON.stringify(data)
        }).done(function (response) {
            if (response.isSuccess) {
                _bindForms(response.data, response.pageInfo);
            }
        });
    }

    function _bindForms(data, pageInfo) {
        var $body = $("#formList tbody", _$container);
        $body.empty();

        if ($.isArray(data) && data.length > 0) {
            $.each(data, function () {
                var $tr = $(_template).appendTo($body);

                $("td:eq(0)", $tr).text(this.formNo);
                $("td:eq(1)", $tr).text(this.productName);
                $("td:eq(2)", $tr).text(this.count);
                $("td:eq(3)", $tr).text(this.statusName);

                var $td = $("td:eq(4)", $tr);
                if (this.status === "confirmed") {
                    $td.find("div.btn-group").html('<a class="btn btn-primary items">查看批次</a>');
                }
                else if (this.status === "dispatching") {
                    $("a.cancel", $td).remove();
                }
                else if (this.status === "cancelled") {
                    $td.empty();
                }

                $tr.data("id", this.id);
            });
        }

        lims.pager.bindPager(pageInfo);
    }

    function cancel(id, callback) {
        var data = {
            id: id
        };

        $.ajax({
            url: window.baseUrl + "Vendor/Dispatch/Cancel",
            contentType: "application/json",
            type: "post",
            dataType: "json",
            data: JSON.stringify(data)
        }).done(function (response) {
            if (response.isSuccess) {
                if (response.message) {
                    lims.window.alert(response.message);
                }
                else {
                    callback && callback();
                }
            }
        });
    }

    lims.dispatchIndex = {
        init: function () {
            _init();
        }
    }
})();