(function () {
    var lims = window.lims = window.lims || {};

    if (lims.orderQueryIndex) {
        return;
    }


    var _$container, _TEMPLATE, _condition, _pageIndex;
    function _init() {
        _$container = $("#orderQueryIndex");
        _TEMPLATE = _$("#itemTemplate").html();

        _$("#beginDate").datepicker();
        _$("#endDate").datepicker();

        lims.util.numericBinder(_$("#formNo"), { point: 0 });

        _$("#btnQuery").click(function () {
            _pageIndex = 0;
            _condition = {
                content: _$("#formNo").val(),
                beginDate: _$("#beginDate").datepicker("getDate"),
                endDate: _$("#endDate").datepicker("getDate")
            };

            _query();
        });

        _$("#btnMerge").click(function () {
            var ids = [], $this = $(this);

            var $checkboxs = _$("#formList input:checked");
            $.each($checkboxs, function () {
                var $this = $(this);
                ids.push($this.parents("tr").data("id"));
            });

            $this.next().addClass("hidden");
            if (ids.length == 0) {
                $this.next().removeClass("hidden");
                return;
            }

            _dispatch(ids);
        });

        _$("#formList").on("click", "a.dispatch", function () {
            var id = $(this).parents("tr").data("id");
            if (id) {
                _dispatch([id]);
            }
        });

        lims.pager.init(_$("#pageList"), function (newPageIndex) {
            _pageIndex = newPageIndex;
            _query();
        });

        _query();
    }

    function _$(id) {
        return $(id, _$container);
    }

    function _query() {
        var data = {
            condition: _condition,
            pager: { pageIndex: _pageIndex }
        };

        $.ajax({
            url: window.baseUrl + "Vendor/OrderQuery/Query",
            contentType: "application/json",
            type: "post",
            dataType: "json",
            data: JSON.stringify(data)
        }).done(function (response) {
            if (response.isSuccess) {
                _bindData(response.data, response.pageInfo);
            }
        });
    }

    function _bindData(data, pageInfo) {
        var $body = _$("#formList tbody");
        $body.empty();

        if ($.isArray(data) && data.length > 0) {
            $.each(data, function () {
                var $tr = $(_TEMPLATE).appendTo($body);

                $("td:eq(1)", $tr).text(this.formNo);
                $("td:eq(2)", $tr).text(this.unitName);
                $("td:eq(3)", $tr).text(this.productName);
                $("td:eq(4)", $tr).text(this.count);
                $("td:eq(5)", $tr).text(this.price);
                $("td:eq(6)", $tr).text(this.sum);
                $("td:eq(7)", $tr).text(this.donateCount);
                $("td:eq(8)", $tr).text(lims.util.formatDate(this.expiredDate));
                $("td:eq(9)", $tr).text(lims.util.formatDate(this.expectDate));
                $("td:eq(10)", $tr).text(this.statusName);

                $tr.data("id", this.id);
            });
        }

        lims.pager.bindPager(pageInfo);
    }

    function _dispatch(ids) {
        lims.window.pop("Vendor/OrderQuery/Dispatch?ids=" + ids.join(","), function () {
            if (lims.dispatchEdit) {
                lims.dispatchEdit.onClose(function () {
                    _query();
                });
            }
        });
    }

    lims.orderQueryIndex = {
        init: function () {
            _init();
        }
    };
})();