var CXFB = '撤销发布';
var LJFB = '立即发布';
var BCCG = '保存草稿';
var GXBW = '更新博文';

var delItem = '<div class="ember-view gh-blognav-item" >' +
                    '<span class="gh-blognav-grab icon-grab">' +
                        '<span class="sr-only">重新排序</span>' +
                    '</span>' +
                    '<div class="gh-blognav-line">' +
                        '<span class="gh-blognav-label ember-view" >' +
                            '<input class="ember-view ember-text-field gh-input"  autofocus="" type="text" placeholder="标题">' +

                        '</span><span class="gh-blognav-url ember-view" >' +
                            '<input class="ember-view ember-text-field gh-input" type="text">' +

                        '</span>' +
                    '</div>' +
                    '<button class="gh-blognav-delete" type="button">' +
                        ' <i class="icon-trash"><span class="sr-only">删除</span></i>' +
                    '</button>' +
                '</div>';
var addItem = '<div class="ember-view gh-blognav-item ignore-elements" >' +
                    '<span class="gh-blognav-grab icon-grab">' +
                        '<span class="sr-only">重新排序</span>' +
                    '</span>' +
                    '<div class="gh-blognav-line">' +
                        '<span class="gh-blognav-label ember-view" >' +
                            '<input class="ember-view ember-text-field gh-input"  autofocus="autofocus" value="标题"  type="text" placeholder="标题">' +

                        '</span><span class="gh-blognav-url ember-view" >' +
                            '<input class="ember-view ember-text-field gh-input" type="text" value = "http://' + $('#host').val() + '/">' +
                        '</span>' +
                    '</div>' +
                    '<button class="gh-blognav-add" type="button">' +
                        ' <i class="icon-add2"><span class="sr-only">添加</span></i>' +
                    '</button>' +
                '</div>';


$(function () {
    /* 在textarea处插入文本--Start */
    (function ($) {
        $.fn.extend({
            insertContent: function (myValue, t) {
                var $t = $(this)[0];
                if (document.selection) { // ie
                    this.focus();
                    var sel = document.selection.createRange();
                    sel.text = myValue;
                    this.focus();
                    sel.moveStart('character', -l);
                    var wee = sel.text.length;
                    if (arguments.length == 2) {
                        var l = $t.value.length;
                        sel.moveEnd("character", wee + t);
                        t <= 0 ? sel.moveStart("character", wee - 2 * t - myValue.length) : sel.moveStart("character", wee - t - myValue.length);
                        sel.select();
                    }
                } else if ($t.selectionStart
                 || $t.selectionStart == '0') {
                    var startPos = $t.selectionStart;
                    var endPos = $t.selectionEnd;
                    var scrollTop = $t.scrollTop;
                    $t.value = $t.value.substring(0, startPos)
                     + myValue
                     + $t.value.substring(endPos, $t.value.length);
                    this.focus();
                    $t.selectionStart = startPos + myValue.length;
                    $t.selectionEnd = startPos + myValue.length;
                    $t.scrollTop = scrollTop;
                    if (arguments.length == 2) {
                        $t.setSelectionRange(startPos - t,
                         $t.selectionEnd + t);
                        this.focus();
                    }
                } else {
                    this.value += myValue;
                    this.focus();
                }
            }
        })
    })(jQuery);
    /* 在textarea处插入文本--Ending */
});

var activeMenu = "a_save";

var isPost = false;
var isPre = false;

var idVal = 0;

$(document).ready(function () {

    $('#postsCommentPlugin').on('input', function () {
        try
        {
            $('#postsCommentPluginValue').val(window.btoa($('#postsCommentPlugin').val()));
        }catch(e){};
       
    });

    $('#forgotten').on('click', function () {
        var email = $('#Email').val();
        if (CheckMail(email)) {
            var data = JSON.stringify({
                emailName: email
            });
            $.ajax({
                type: "POST",
                url: "/Account/ForgotPassword",
                dataType: 'json',
                data:data,
                contentType: 'application/json',
                success: function (msg) {
                    if(msg)
                        alert('请前往' + email + '按照邮件提示完成密码重置！');
                    else
                        alert('邮件发送失败！');
                }
            });
        } else {
            alert('请输入正确的邮件地址')
        }
    });
    $('#aimg').html(getImgUploadSection(""));

    $("#action").on('click', function () {
        var c = $.trim(this.textContent);
        switch (c) {
            case GXBW:
            case BCCG:
                saveArticle();
                break;
            case LJFB:
                saveArticle(1);
                break;
            case CXFB:
                saveArticle(0);
                break;
        }
    });

    $("#toList").on('click', function () {
        window.location = "/manage";
    });

    $(".btn_pre").on('click', function () {
        $("#s_md").removeClass("active");
        $("#s_pre").addClass("active");
        $(".btn_pre").addClass("active");
        $(".btn_md").removeClass("active");
        preHtml();
    });

    $(".btn_md").on('click', function () {
        $("#s_pre").removeClass("active");
        $("#s_md").addClass("active");
        $(".btn_md").addClass("active");
        $(".btn_pre").removeClass("active");
    });

    $(".btn_list").on('click', function () {
        window.location = '/manage/';
    });

    $('.gh-mobilemenu-button').on('click', function () {
        $('body').addClass('mobile-menu-expanded');
        $('#gh-viewport').addClass('mobile-menu-expanded');

    });

    $('.gh-menu-toggle').on('click', function () {
        $('body').removeClass('mobile-menu-expanded');
        $('#gh-viewport').removeClass('mobile-menu-expanded');
    });

    $(".m").on('click', function (e) {
        var c = $.trim(this.textContent);
        $('#action').html(c);
        if (this.id == "a_pub") {
            $('#action').removeClass('btn-blue');
            $('#action').addClass('btn-red');
            $('#save').removeClass('btn-blue');
            $('#save').addClass('btn-red');
        } else {
            $('#action').removeClass('btn-red');
            $('#action').addClass('btn-blue');
            $('#save').removeClass('btn-red');
            $('#save').addClass('btn-blue');
        }
        $(this).addClass('active');
        $('#' + activeMenu).removeClass('active');
        activeMenu = this.id;
        saveClick()
    });

    $('#btn_del').on('click', function (e) {
        saveClick();
        $('#title_del').text($('#Article_title').val())
        $('#del_view').show();
    });

    $('#cancel_del').on('click', function (e) {
        $('#del_view').hide()
    });

    $('#ok_del').on('click', function (e) {
        delArticle()
    });

    $('#MetaTitle').on('input', function (e) {
        loadSeo();
    });

    $('#MetaDescription').on('input', function (e) {
        loadSeo();
    });

    $('.is').on('click', function (e) {
        if ($('#IsSeparate').attr("checked")) {
            $('#IsSeparate').attr("checked", false);
        } else {
            $('#IsSeparate').attr("checked", "checked");
        }

    });

    $('.if').on('click', function (e) {
        if ($('#IsRecommend').attr("checked")) {
            $('#IsRecommend').attr("checked", false);
        } else {
            $('#IsRecommend').attr("checked", "checked");
        }

    });

    $('#cancel_insert').on('click', function (e) {
        $('#insert_img_view').hide();
    });

    $('#ok_insert').on('click', function (e) {
        $('#insert_img_view').hide();
        var host = $('#Host').val();
        var v = $('#imgValue0').val();
        v = '\n![插入的图片](http://' + host + v + ')\n';
        $('#postContent').insertContent(v);

        document.getElementById("postContent").oninput();

    });

    $('#ok_insert_cover').on('click', function (e) {
        $('#insert_img_view').hide();      
        $('#cover').val($('#imgValue0').val());
        $('.js-upload-target').attr('src', $('#imgValue0').val())
        $('#cover_img').show();
    });

    $('.js-modal-cover').on('click', function (e) {
        insertImg()
    });

    $('.del_cover').on('click', function (e) {
        $('#cover_img').hide();
        $('#cover').val('');
    });     

    $("#postContent").hover(
        function () {
            isPost = true;
            isPre = false;
        },
        function () {
            isPost = false;
            isPre = false;
        }
    );

    $("#preSec").hover(
        function () {
            isPost = false;
            isPre = true;
        },
        function () {
            isPost = false;
            isPre = false;
        }
    );

    $('#postContent').on('scroll', function () {
        if (isPost) {
            d = document.getElementById('preSec');
            per = this.scrollTop / (this.scrollHeight - this.clientHeight);
            $(d).scrollTop((d.scrollHeight - d.clientHeight) * per);
        }        
    });

    $('#preSec').on('scroll', function () {
        if (isPre) {
            d = document.getElementById('postContent');
            per = this.scrollTop / (this.scrollHeight - this.clientHeight);
            $(d).scrollTop((d.scrollHeight - d.clientHeight) * per);
        }
    });

    $('#btnSave').on('click', function (e) {
        $('#generalId').val(1);
        $('#settings-general').submit();

    });

    $('#btnNavSave').on('click', function (e) {
        busyAnimation($('#btnNavSave'));
        var i = 0;
        var datas = '{"navigations":[';
        $('.gh-blognav-item:not(.ignore-elements)').each(function (e) {

            datas += JSON.stringify({
                Title: $($(this.childNodes).find('input')[0]).val(),
                Link: $($(this.childNodes).find('input')[1]).val(),
                Sequence: i++
            });
            datas += ',';
        });
        datas = datas.substring(0, datas.length - 1);
        datas += ']}';
        $.ajax({
            type: "POST",
            url: "/manage/Navigation",
            dataType: 'json',
            data: datas,
            contentType: 'application/json',
            success: function (msg) {
                unBusyAnimation($('#btnNavSave'), '保存');
            }
        });

    });
    
    window.onresize = function () {
        initList();

    }

    bindAction();
    if (navList)
    {
        Sortable.create(navList, {
            handle: '.gh-blognav-grab',
            animation: 150,

            // dragging ended
            onEnd: function (/**Event*/evt) {
                evt.oldIndex;  // element's old index within parent
                evt.newIndex;  // element's new index within parent
            },
            onUpdate: function (/**Event*/evt) {
                var itemEl = evt.item;  // dragged HTMLElement
                // + indexes from onEnd
            },
            onSort: function (/**Event*/evt) {
                var itemEl = evt.item;
                // same properties as onUpdate
            },
            onMove: function (/**Event*/evt) {
                // Example: http://jsbin.com/tuyafe/1/edit?js,output
                evt.dragged; // dragged HTMLElement
                evt.draggedRect; // TextRectangle {left, top, right и bottom}
                if (evt.related.classList.contains('ignore-elements')) return false; // HTMLElement on which have guided
                evt.relatedRect; // TextRectangle
                // return false; — for cancel
            }



        });
    }   

    initStatue('AbstractImg');
    initList();
});

function IsURL(str_url) {
    var strRegex = "^((https|http|ftp|rtsp|mms)?://)"
    + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //ftp的user@ 
          + "(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP形式的URL- 199.194.52.184 
          + "|" // 允许IP和DOMAIN（域名）
          + "([0-9a-z_!~*'()-]+\.)*" // 域名- www. 
          + "([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\." // 二级域名 
          + "[a-z]{2,6})" // first level domain- .com or .museum 
          + "(:[0-9]{1,4})?" // 端口- :80 
          + "((/?)|" // a slash isn't required if there is no file name 
          + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";
    var re = new RegExp(strRegex);
    //re.test()
    if (re.test(str_url)) {
        return (true);
    } else {
        return (false);
    }
}

function bindAction() {
    $('.gh-blognav-add').one('click', function (e) {
        var inputs = $(this.parentElement).find('input');
        title = $(inputs[0]).val();
        link = $(inputs[1]).val();
        if (title == undefined || title == '' || title.length == 0) {
            alert('请输入正确的标题！')
            bindAction();
            return;
        }
        if (!IsURL(link)) {
            alert('请输入正确的超链接地址！')
            bindAction();
            return;
        }
        $(this.parentElement).removeClass('ignore-elements');
        $(this).removeClass('gh-blognav-add');
        $(this).addClass('gh-blognav-delete');
        $(this).html('<i class="icon-trash"><span class="sr-only">删除</span></i>');
        $('#navList').append(addItem);
        bindAction();
    });
    $('.gh-blognav-delete').one('click', function (e) {
        $(this.parentElement).addClass('del')
        $(this.parentElement.parentElement).children('.del').remove();
    });
}

function busyAnimation(ele) {
    ele.css('width:56px;height:36px')
    ele.html('<span class="spinner"></span>')
}

function unBusyAnimation(ele, text) {
    ele.html(text)
}

function initList() {
    if (window.innerWidth < 897) {
        $('.permalink').each(function () {
            var h = $(this).attr('href');
            h = h.replace('index', 'editor');
            $(this).attr('href', h);
        });
    } else {
        $('.permalink').each(function () {
            var h = $(this).attr('href');
            h = h.replace('editor', 'index');
            $(this).attr('href', h);

        });
    }
}

function loadSeo() {
    var mt = ''; var md = '';
    if ($('#MetaTitle').val()) {
        mt = $('#MetaTitle').val()
        $('#mtCount').text(mt.length);
    } else {
        mt = $('#Article_title').val();
    }
    if ($('#MetaDescription').text()) {
        md = $('#MetaDescription').text()
        $('#mdCount').text(md.length);
    } else {
        md = getAbstract();
    }
    $('.seo-preview-title').text(mt)
    $('.seo-preview-link').text($('#url').val())
    $('.seo-preview-description').text(md)
}

function getAbstract() {
    var c = $('#preHtml').val();
    var l = c.length;
    if (l > 156) {
        return c.substr(0, 156);
    } else {
        return c
    }
}

function delArticle() {
    var data = JSON.stringify({
        ArticleId: $("#ArticleId").val()
    });
    $.ajax({
        type: "POST",
        url: "/manage/DeleteArtice",
        dataType: 'json',
        data: data,
        contentType: 'application/json',
        success: function (msg) {
            window.location('/manage');
        }
    });
}

function saveArticle(isPub) {
    run();
    if (isPub == undefined)
        isPub = $('#IsPublish').val();
    var data = JSON.stringify({
        Id: $("#ArticleId").val(),
        Title: $("#Article_title").val(),
        Content: $("#postContent").val(),
        Abstract: getAbstract(),
        Tags: $("#tags").val(),
        MetaTitle: $("#MetaTitle").val(),
        MetaDescription: $("#MetaDescription").val(),
        IsPublish: isPub,
        IsSeparate: getChecked($('#IsSeparate').attr("checked")),
        IsRecommend: getChecked($('#IsRecommend').attr("checked")),
        AbstractImg: $("#AbstractImg").val()
    });
    $.ajax({
        type: "POST",
        url: "/manage/SaveArtice",
        dataType: 'json',
        data: data,
        contentType: 'application/json',
        success: function (msg) {
            if ($('#ArticleId').val() == 0)
            {
                host = $('#Host').val();
                url = 'http://' + host + '/home/article/' + msg.Id
                $('#url').val(url);
                $('.ghost-url-preview').val(url);
                $('.post-view-link').attr('href', url);
            }
            $("#ArticleId").val(msg.Id);
            $("#IsPublish").val(msg.IsPublish ? '1' : '0');
            $("#AbstractImg").val(msg.AbstractImg);
            $("#post-setting-date").val(msg.CreateDate);                      
            unRun();
            initStatue('AbstractImg');
        }
    });
}

function getChecked(e) {
    if (e == 'checked') {
        return 1
    } else {
        return 0
    }
}

var tmpHtml;

function run() {
    tmpHtml = $('#action').html();
    $('#action').html('<span class="spinner"></span>')
}

function unRun() {
    $('#action').html(tmpHtml);
}

function initStatue(abstractImg) {
    if ($('#ArticleId').val() == 0) {
        initNoSave();
    }
    else {
        if ($('#IsPublish').val() == '1') {
            initSavePub();
        } else {
            initSaveNoPub();
        }
    }
    $('#action').html($('#' + activeMenu).text());
    if ($("#" + abstractImg).val() != '') {
        showFile($("#" + abstractImg).val(), 'upimg', 'AbstractImg'); //AbstractImg
    }
}

function setStatue(e, c) {
    c = '<a href="javascript:;" >' + c + '</a>'
    e.html(c)
}

function initNoSave() {
    setStatue($('#a_pub'), LJFB);
    setStatue($('#a_save'), BCCG);
}

function initSaveNoPub() {
    setStatue($('#a_pub'), LJFB);
    setStatue($('#a_save'), GXBW);

}

function initSavePub() {
    setStatue($('#a_pub'), CXFB);
    setStatue($('#a_save'), GXBW);

}

function saveClick() {
    if ($('#save').hasClass('closed')) {
        $('#save').removeClass('closed')
        $('#save').addClass('open')
        $('#saveMenu').removeClass('closed fade-out')
        $('#saveMenu').addClass('open fade-in-scale')
    } else {
        $('#save').removeClass('open')
        $('#save').addClass('closed')
        $('#saveMenu').removeClass('open fade-in-scale')
        $('#saveMenu').addClass('closed fade-out')

    }
}

function openSetting() {
    $('body').addClass('settings-menu-expanded');
    $('.gh-viewport').addClass('settings-menu-expanded');
}

function closeSetting() {
    $('body').removeClass('settings-menu-expanded');
    $('.gh-viewport').removeClass('settings-menu-expanded');
}

function openSEO() {
    $('#seo').removeClass('settings-menu-pane-out-right');
    $('#setting').removeClass('settings-menu-pane-in');
    $('#setting').addClass('settings-menu-pane-out-left');
    loadSeo();
}

function closeSEO() {
    $('#seo').addClass('settings-menu-pane-out-right');
    $('#setting').addClass('settings-menu-pane-in');
    $('#setting').removeClass('settings-menu-pane-out-left');
}

function closeMkHelp() {
    $('#mkHelp').hide();
}

function openMkHelp() {
    $('#mkHelp').show();
}

var converter
try {
    converter = new showdown.Converter();
} catch (e) {
    converter = undefined
}


function preHtml() {
    if (converter != undefined) {
        var tt = converter.makeHtml($("#postContent").val(), true);
        document.getElementById("preHtml").innerHTML = tt;
        $('#contentCount').text($("#preHtml").text().length + " 个字");
    } else {
        alert("转换器加载失败");
    }
}

function getImgUploadSection(idNum) {
    var v = $('imgValue' + idNum).val();
    var ih = '<section class="ember-view js-post-image-upload image-uploader" id="upimg' + idNum + '" style="height: auto; min-height: 0px; opacity: 100;" data-uploaderui="true">' +
            '<span class="media" style="opacity: 0;">' +
                '<span class="hidden">上传图片</span>' +
            '</span>' +
            '<img width="301" height="169" class="js-upload-target" style="display: none;" src="">' +
            '<div class="description" style="opacity: 100;">' +
                '为博文设置图片<strong></strong>' +
            '</div>' +
            '<input name="uploadimage" id="ai' + idNum + '" onchange="uploadFile(\'' + idNum + '\')" class="gh-input js-fileupload main fileupload" type="file" data-url="upload">' +
            '<div class="js-upload-progress progress progress-success active" role="progressbar" id="pb' + idNum + '" aria-valuemin="0" aria-valuemax="100" style="opacity: 100; display:none;">' +
                '<div class="js-upload-progress-bar bar" id="son' + idNum + '" style="width:0%"></div>' +
            '</div>' +
            '<a title="添加图片地址（URL）" class="image-url" onclick="addImgPath(\'' + idNum + '\')">' +
                '<i class="icon-link">' +
                    '<span class="hidden">URL</span>' +
                '</i>' +
            '</a>' +
            '<input type="hidden" id="imgValue' + idNum + '" value="' + v + '"/>' +
        '</section>';
    return ih;
}

function uploadFile(idNum) {
    //$('.description').hide();
    $('#pb' + idNum).show();

    var mypic = document.getElementById('ai' + idNum).files[0];
    var fd = new FormData();
    fd.append("ai" + idNum, mypic);
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            $('#pb' + idNum).hide();
            var path = $.parseJSON(xhr.responseText)
            showFile(path.msg, idNum)//AbstractImg
        }
    }
    //侦查当前附件上传情况
    xhr.upload.onprogress = function (evt) {
        var loaded = evt.loaded;
        var tot = evt.total;
        var per = Math.floor(100 * loaded / tot);  //已经上传的百分比
        var son = document.getElementById('son' + idNum);
        son.style.width = per + "%";
    }
    xhr.open("post", "/manage/Upload");
    xhr.send(fd);
}

function saveImgUrl(idNum) {
    showFile($('#imgUrl' + idNum).val(), idNum);
    $('#upimg' + idNum).removeClass('image-uploader-url');
}

function showUploadImg(idNum) {
    delImg(idNum);
    $('#upimg' + idNum).removeClass('image-uploader-url');
}

function addImgPath(idNum) {
    var ih = '<div class="js-url"><input class="url js-upload-url gh-input"  type="url" placeholder="http://" id="imgUrl' + idNum + '"><button class="btn btn-blue js-button-accept gh-input" onclick="saveImgUrl(\'' + idNum + '\')">保存</button></div><div class="description" style="display: none; opacity: 100;">为博文设置图片<strong></strong></div>' +
            '<input name="uploadimage"  class="gh-input js-fileupload main fileupload right" type="file" data-url="upload" >' +
            '<a title="添加图片" class="image-upload icon-photos" onclick="showUploadImg(\'' + idNum + '\')"><span class="hidden">上传</span></a>';
    $('#upimg' + idNum).html(ih);
    $('#upimg' + idNum).addClass('image-uploader-url');
}

function delImg(idNum) {
    var ih = getImgUploadSection(idNum);
    $('#aimg' + idNum).html(ih);
    $('#upimg' + idNum).addClass('image-uploader');
    $('#upimg' + idNum).removeClass('pre-image-uploader');
}

function showFile(filePath, idNum) {
    var ih = ' <img width="301" height="272" class="js-upload-target" style="display: block;" src="' + filePath + '">' +
               '<div class="description" style="display: none; opacity: 100;">为博文设置图片<strong></strong></div>' +
               '<input name="uploadimage" class="gh-input js-fileupload main fileupload" type="file" data-url="upload">' +
               '<a title="删除" class="image-cancel icon-trash js-cancel" onclick="delImg(\'' + idNum + '\')">' +
               '<span class="hidden">删除</span>' +
               '</a>' +
               '<input type="hidden" id="imgValue' + idNum + '" value=""/>';
    $('#upimg' + idNum).html(ih);
    $('#upimg' + idNum).removeClass('image-uploader');
    $('#upimg' + idNum).addClass('pre-image-uploader');
    $("#imgValue" + idNum).val(filePath); //AbstractImg
    if (idNum == "") {
        $('#AbstractImg').val(filePath);
    }
}

function insertImg() {
    $('#aimg0').html(getImgUploadSection(0));
    $('#insert_img_view').show();
}

function CheckMail(mail) {
    var filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (filter.test(mail)) return true;
    else {        
        return false;
    }
}



