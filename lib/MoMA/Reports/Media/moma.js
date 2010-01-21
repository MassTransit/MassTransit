function toggle (name, img)
{
    var element = document.getElementById (name);
    
    if (element.style.display == 'none')
        element.style.display = '';
    else
        element.style.display = 'none';

    var img_element = document.getElementById (img);

    if (img_element.src.indexOf ('minus.png') > 0)
        img_element.src = 'Media/plus.png';
    else
        img_element.src = 'Media/minus.png';
}

