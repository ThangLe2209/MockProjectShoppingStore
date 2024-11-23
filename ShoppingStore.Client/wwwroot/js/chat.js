"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:7045/chatHub").build();
var newTotal = 0;

connection.on("ReceiveMessage", function (userName, successOrder) {
    Date.prototype.addHours = function (h) {
        this.setHours(this.getHours() + h);
        return this;
    }

    var resultData = JSON.parse(successOrder);
    //console.log(userName, resultData);

    newTotal++;
    $("#admin-orders-tag").html(`Orders (${newTotal})`);

    if ($('#myAdminOrderListTable').length) {
        // do stuff
        //console.log("hihi")
        $('#myAdminOrderListTable').prepend(`
             <tr style="font-weight: 800">
                <th scope="row" style="padding: 8px 10px;">${resultData.Id}</th>
                <td style="padding: 8px 10px;">${resultData.OrderCode}</td>
                <td style="padding: 8px 10px;">${resultData.CouponCode == null ? "" : resultData.CouponCode}</td>
                <td style="padding: 8px 10px;" class="dt-type-numeric">${resultData.ShippingCost} $</td>
                <td style="padding: 8px 10px;">${resultData.UserName}</td>
                <td style="padding: 8px 10px;" class="sorting_1">${(new Date(resultData.CreatedDate)).addHours(7).toISOString().slice(0, 19).replace(/-/g, "/").replace("T", " ") } </td>
                <td style="padding: 8px 10px;">
                        <span class="text text-success">New Order</span>
                </td>
                <td style="padding: 8px 10px;">
                    <a class="btn btn-warning btn-sm" href="/Admin/Order/ViewOrder?ordercode=${resultData.OrderCode}">View Order</a>
                    <a class="btn btn-danger btn-sm confirmDeletion" href="/Admin/Order/Delete/${resultData.Id}">Delete</a>
                </td>
            </tr>
        `);
    }
});


function isSuccessOrder(successOrder) {
    connection.start().then(function () {
        if (successOrder != "") {
            //console.log("hihi", typeof successOrder, JSON.parse(successOrder));
            //var result = JSON.parse(successOrder);
            var result = successOrder;
            connection.invoke("JoinChat", result).catch(function (err) {
                return console.error(err.toString());
            });
        }
    }).catch(function (err) {
        return console.error(err.toString());
    });

}
