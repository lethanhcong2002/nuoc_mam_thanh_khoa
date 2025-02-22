let thisPage = 1;
let limit = 3;
let list = document.querySelectorAll('.mainnews .p3');

function loadItem() {
    let beginGet = limit * (thisPage - 1);
    let endGet = limit * thisPage - 1;
    list.forEach((item, key) => {
        if (key >= beginGet && key <= endGet) {
            item.style.display = 'block';
        } else {
            item.style.display = 'none';
        }
    });
}
loadItem();

function listPage() {
    let count = Math.ceil(list.length / limit);
    let pagination = document.querySelector('.pagination');
    pagination.innerHTML = '';

    if (thisPage != 1) {
        let prev = document.createElement('li');
        prev.classList.add('page-item');
        prev.innerHTML = `<a href="javascript:void(0);" class="page-link" aria-label="Previous" onclick="changePage(${thisPage - 1})"><span aria-hidden="true">«</span></a>`;
        pagination.appendChild(prev);
    }

    for (let i = 1; i <= count; i++) {
        let newPage = document.createElement('li');
        newPage.classList.add('page-item');
        if (i === thisPage) {
            newPage.classList.add('active');
        }
        newPage.innerHTML = `<a href="javascript:void(0);" class="page-link" onclick="changePage(${i})">${i}</a>`;
        pagination.appendChild(newPage);
    }

    if (thisPage != count) {
        let next = document.createElement('li');
        next.classList.add('page-item');
        next.innerHTML = `<a href="javascript:void(0);" class="page-link" aria-label="Next" onclick="changePage(${thisPage + 1})"><span aria-hidden="true">»</span></a>`;
        pagination.appendChild(next);
    }
}
listPage();

function changePage(i) {
    thisPage = i;
    loadItem();
    listPage();
}
