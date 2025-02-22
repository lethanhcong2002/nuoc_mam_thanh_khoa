/* Sự kiện thay đổi trong PickerSelect Price */
var listprice1 = document.getElementById('listprice1');
var listprice2 = document.getElementById('listprice2');

function updateListprice2() {
    var selectedValue1 = parseInt(listprice1.value);

    // Loop through listprice2 options and hide options less than selectedValue1
    var options = listprice2.options;
    for (var i = 0; i < options.length; i++) {
        var optionValue = parseInt(options[i].value);
        if (optionValue < selectedValue1) {
            options[i].setAttribute('hidden', true);
        } else {
            options[i].removeAttribute('hidden');
        }
    }

    // If the selected option in listprice2 is hidden, select the first visible option
    if (parseInt(listprice2.value) < selectedValue1 || listprice2.options[listprice2.selectedIndex].hasAttribute('hidden')) {
        for (var i = 0; i < options.length; i++) {
            if (!options[i].hasAttribute('hidden')) {
                listprice2.value = options[i].value;
                break;
            }
        }
    }
}

// Add event listener to listprice1
listprice1.addEventListener('change', updateListprice2);

// Initialize listprice2 options on page load
updateListprice2();