function makeDataTable() {
    var datatables = document.querySelectorAll('.datatable')
    datatables.forEach(datatable => {
        new simpleDatatables.DataTable(datatable, {
            searchable: false,
            paging: false
        });
    })
}