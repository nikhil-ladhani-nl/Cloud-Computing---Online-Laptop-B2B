//Get All Products
var ViewModel = function WriteResponse(products) {
    alert("Success");
}
var ViewModel = function () {
    var self = this;
    self.products = ko.observableArray();
    self.error = ko.observable();

    var productsUri = 'http://localhost:52581/api/Products';

    function ajaxHelper(uri, method, data) {
        self.error(''); // Clear error message
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        }).fail(function (jqXHR, textStatus, errorThrown) {
            self.error(errorThrown);
        });
    }

    function GetAllProducts() {
        ajaxHelper(productsUri, 'GET').done(function (data) {
            self.products(data);
        });
    }

    // Fetch the initial data.
    GetAllProducts();

    ko.applyBindings(new ViewModel());

};