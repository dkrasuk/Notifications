import dateFormat from 'dateformat';
import logger from '../services/loggerService';

let _template = `<div class="list-group"></div>`;
let _itemTemplate =
                `<div class="list-group-item">
                    <span class="pull-left m-r thumb-sm">
                      <input type="checkbox" value="" class="notify-checkbox"/>
                    </span>
                    <div><span class="notify-type-ico"></span><b class="notify-title"></b></div> 
                    <span class="clear block m-b-none" >
                      <span class="notify-text"></span><br/>
                      <small class="text-muted notify-date"></small>
                    </span>
                    <div class="mark-as-read-btn" title="Отметить как прочитаное"><div class="mark-as-read-btn-ico"></div></div>
                  </div>`;

let _config;
let _notificationService;
let $element = null;
let filter = {
    isShowReaded: false,
    groupBy: null
};

let events = {};

let _notifications = [];
let groups = {};

function getEvent(name){
    return events[name] || [];
}

function callErrorEvent(error){
    let event = getEvent('error');
    for(let i = 0; i < event.length; i++) {
        event[i].call(this, _config.messages.commonHttpError, error);
    }
}

function getGroupList(item) {
    let group = item[filter.groupBy];
    if (filter.groupBy && filter.groupBy === 'CreatedDate'){
        let index = group.indexOf('T') || group.indexOf(' ');
        let dateArray = group.substr(0, index).split('-');
        group = dateArray[2] + '.'  + dateArray[1] + '.' + dateArray[0];
    }
    return (groups[group]
            || (groups[group]
                 = $(`<div class="list-group-block" >
                                <div class="block-head"><strong>` + group + `</strong></div>
                                <div class="block-body"></div>
                             </div>`)
                    .prependTo($element).find('.block-body')));
}

function renderNotifications(data) {
    if(!filter.groupBy) {
        for (let i = (data.length - 1); i >= 0; i--) {
            renderNotificationItem(data[i], false, $element)
        }
    } else {
        groups = {};
        for (let i = (data.length - 1); i >= 0; i--) {
            renderNotificationItem(
                data[i],
                false,
                getGroupList(data[i])
            );
        }
    }
}

function renderNotifyIcoByType(type) {
    let result = '<span class="';
    switch (type){
        case 'error':
            result += 'glyphicon glyphicon-remove-circle text-danger';
            break;
        case 'warn':
            result += 'glyphicon glyphicon-warning-sign text-warning';
            break;
        case 'info':
            result += 'glyphicon glyphicon-info-sign text-primary';
            break;
        case 'success':
            result += 'glyphicon glyphicon-ok-circle text-success';
            break;
        default:
            break;
    }
    return result + '" ></span>'
}

function renderNotificationItem(message, animate = false, element){
    if (!filter.isShowReaded && message.IsReaded){
        return;
    }
    element = element || $element;
    let item = $(_itemTemplate);
    item.attr('data-notify-id', message.Id);

    item.find('.notify-type-ico').html(renderNotifyIcoByType(message.Type));
    item.find('.notify-title').html(message.Title);
    item.find('.notify-text').html(message.Body);

    item.find('.notify-date').html(dateFormat(new Date(message.CreatedDate), 'dd.mm.yyyy HH:MM:ss'));
    item.find('input.notify-checkbox').val(message.Id);

    item.data('notification', message);

    item.on('click', function(e){
       // e.preventDefault();
    });
    if (!message.IsReaded){
        item.addClass('new');
        item.find('.mark-as-read-btn').on('click', function(e){
            e.preventDefault();
            _notificationService.markAsRead(message.Id).then(
                function(){},
                function(error){
                    callErrorEvent(error);

                    logger.error(error);
                }
            );
        });
    }
    if (animate){
        item.hide();
        item.prependTo(element);
        item.fadeIn("slow");
    } else {
        element.prepend(item);
    }
}

function markAsRead(id){
    for(let i = 0; i < id.length; i++){
        let item = $element.find('*[data-notify-id="'+id[i]+'"]');
        let message = item.data('notification');
        message.IsReaded = true;
        item.removeClass('new');
        if(!filter.isShowReaded){
            item.fadeOut("slow", function () {
                item.remove();
            });
        }
    }
}

function getSelectedId() {
    return $element.find('input.notify-checkbox:checked').map(function () {
        return parseInt(this.value);
    }).get();
}

class NotificationListComponent {
    constructor(config, notificationService){
        $element = $(_template);
        _notificationService = notificationService;
        _config = config;
    }

    updateList(list){
        $element.html('');
        if(list){
            renderNotifications(list);
        } else {
            _notificationService.getList(filter.isShowReaded).then(
                function (data) {
                    _notifications = data;
                    renderNotifications(data);
                },
                function (error) {
                    callErrorEvent(error);
                    logger.error(error);
                }
            );
        }
    }

    selectAllNotifications(){
        $element.find('.list-group-item .notify-checkbox').attr('checked', true);
    }
    unSelectAllNotifications(){
        $element.find('.list-group-item .notify-checkbox').attr('checked', false);
    }

    addNotificationToList(message){
        _notifications.push(message);
        if (!filter.groupBy) {
            renderNotificationItem(message, true);
        } else {
            renderNotificationItem(message, true, getGroupList(message));
        }
    }

    removeNotificationFromList(id){
        let item = $element.find('[data-notify-id='+id+']');
        item.fadeOut("slow", function () {
            item.remove();
        });
    }

    markAsRead(id){
        markAsRead(id);
    }

    sendSelectedAsRead(){
        let selected = getSelectedId();
        if(selected && selected.length > 0){
            _notificationService.markAsRead(selected);
        }
    }

    groupList(groupBy){
        filter.groupBy = groupBy;
        this.updateList(_notifications);
    }

    on(name, callback){
        if (callback){
            events[name] = events[name] || [];
            events[name].push(callback);
        } else {
            if(events[name] && events[name].length > 0){
                for(let i = 0; i < events[name].length; i++){
                    events[name][i].call(this);
                }
            }
        }
    }

    init(selector){
        $(selector).append($element);
        $element.slimScroll({
            width: '100%',
            height: '300px',
            //scrollBy: '130px',
            scrollTo: '400px',
            size: '8px',
            position: 'right',
            //color: '#ffcc00',
            //alwaysVisible: true,
            distance: '0px',
            //start: 'bottom',
            railVisible: true,
            //railColor: '#222',
            railOpacity: 0.3,
            wheelStep: 10,
            allowPageScroll: false,
            disableFadeOut: false
        });
        $element.slimScroll().bind('slimscroll', function(e, pos){
            console.log("Reached " + pos );
        });
    }
}

export default NotificationListComponent;