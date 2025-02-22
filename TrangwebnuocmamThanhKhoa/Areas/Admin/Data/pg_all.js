let thisPage = 1;
let limit = 5;
let list = document.querySelectorAll('.main_pg .data_pg');
let maxPagesToShow = 5;

function loadItem() {
    let beginGet = limit * (thisPage - 1);
    let endGet = limit * thisPage - 1;
    list.forEach((item, key) => {
        if (key >= beginGet && key <= endGet) {
            item.style.display = 'table-row';
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

    if (thisPage > 1) {
        let first = document.createElement('li');
        first.classList.add('page-item');
        first.innerHTML = `<a href="javascript:void(0);" class="page-link" onclick="changePage(1)">«</a>`;
        pagination.appendChild(first);
    }

    if (thisPage != 1) {
        let prev = document.createElement('li');
        prev.classList.add('page-item');
        prev.innerHTML = `<a href="javascript:void(0);" class="page-link" aria-label="Previous" onclick="changePage(${thisPage - 1})"><span aria-hidden="true">‹</span></a>`;
        pagination.appendChild(prev);
    }

    let startPage = Math.max(1, thisPage - Math.floor(maxPagesToShow / 2));
    let endPage = Math.min(count, startPage + maxPagesToShow - 1);

    startPage = Math.max(1, endPage - maxPagesToShow + 1);

    for (let i = startPage; i <= endPage; i++) {
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
        next.innerHTML = `<a href="javascript:void(0);" class="page-link" aria-label="Next" onclick="changePage(${thisPage + 1})"><span aria-hidden="true">›</span></a>`;
        pagination.appendChild(next);
    }

    if (thisPage < count) {
        let last = document.createElement('li');
        last.classList.add('page-item');
        last.innerHTML = `<a href="javascript:void(0);" class="page-link" onclick="changePage(${count})">»</a>`;
        pagination.appendChild(last);
    }
}
listPage();

function changePage(i) {
    thisPage = i;
    loadItem();
    listPage();
}