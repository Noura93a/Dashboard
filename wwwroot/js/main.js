

function AddNewItems() {
    $('#newproducts').modal('show');
}

function AddNewDamagedproduct() {
    $('#damaged').modal('show');
}

function AddNewDetails() {
    $('#details').modal('show');
}

function update(id) {

    let pId = document.getElementById("Id");
    let name = document.getElementById("Name");
    let description = document.getElementById("Description");

    $.ajax({
        url: '/Home/GetData',
        type: 'POST',
        data: { id: id },

        success: function (result) {

            pId.value = result.id;
            name.value = result.name;
            description.value = result.description;

            console.table(result);
        }

    });


    $('#editproducts').modal('show');

}

function updatedetails(id) {

    let prId = document.getElementById("Id");
    let price = document.getElementById("Price");
    let qty = document.getElementById("Qty");
    let color = document.getElementById("Color");

    $.ajax({
        url: '/Home/GetProductDetails',
        type: 'POST',
        data: { id: id },

        success: function (result) {

            prId.value = result.prId;
            price.value = result.price;
            qty.value = result.qty;
            color.value = result.color;

           

            console.table(result);
        }

    });


    $('#editdetails').modal('show');

}


