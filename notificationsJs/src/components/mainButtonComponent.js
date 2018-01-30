import NotificationListComponent from './notificationsListComponent';
import template from '../views/mainButtonComponent.html';
import logger from '../services/loggerService';
import {notify, browserNotify} from '../shared/notify';

let $element = null;

let _data = {};
let _config = {};
let _listComponent = null;
let _notificationService;
let notificationsCount;
let titleNotifyInterval;

function clearTitleNitifyInterval(title) {
    title = title || $('title');
    title.text(title.text().replace(/^\(\d*\)/, ''));
    clearInterval(titleNotifyInterval);
    titleNotifyInterval = null;
}

function updateNotificationCount(){
    _notificationService.getCount().then(
        function(data){
            updateBadge(data);
        },
        function(error){
            logger.error(error);
        }
    );
}

function titleUpdater(){
    let title = $('title') || $('<title>').appendTo('head');
    let text = title.text();
    if(notificationsCount){
        if (/^\(\d*\)/.test(text)){
            title.text(text.replace(/^\(\d*\)/, ''));
        } else {
            title.text('(' + notificationsCount + ')' + text);
        }
    } else {
        clearTitleNitifyInterval(title);
    }
}

function updateBadge(count){
    let badge = $element.find('.badge');
    let badgeMessage = $element.find('.badge-message');
    let newCount = 0;
    if(count === 0 || count === null || typeof count === 'undefined'){
        badge.hide().text(0);
        notificationsCount = 0;
    } else {
        newCount = parseInt(badge.text()) + count;
        if (newCount <= 0){
            newCount = 0;
            badge.hide();
        } else {
            badge.show();
        }
        badge.text(newCount);
        notificationsCount = newCount;
        titleNotifyInterval = titleNotifyInterval || setInterval(titleUpdater, 1000);
    }
    badgeMessage.text(_config.messages.badgeMessage.replace('{0}', (newCount || 0 ) + ''));
}

function isMenuOpen(){
    return $element.find('.dropdown').eq(0).hasClass('open');
}

function connectToHub() {
    let connection = _notificationService.getHubConnection();
    let hubProxy = connection.createHubProxy(_config.resources.notifications.hubProxy);
    hubProxy.on('sendMessage', function(message) {
        updateBadge(1);
        if (isMenuOpen()){
            _listComponent.addNotificationToList(message);
        } else {
            notify('<b>' + _config.messages.newNotifyTitle + '</b>', message.Body, message.Type);
            let bodyHtml = $('<div>' + message.Body + '</div>');
            browserNotify(_config.messages.newNotifyTitle, bodyHtml.text(), null);
        }
    });
    hubProxy.on('SetNotificationAsReaded', function(id) {
        if (typeof id === 'number'){
            id = [id]
        }
        updateBadge( 0 - id.length);
        if (isMenuOpen()){
            _listComponent.markAsRead(id);
        }
    });

    connection.start().done(function() {
        hubProxy.invoke('register');
    }).fail(function(e){
        logger.error(e);
    });

    $(window).on("unload", function() {
        if(connection.close){
            //connection.close();
        }
    });
}

class MainButtonController{
    constructor(data, config, notificationService){
        _data = data;
        _config = config;
        _notificationService = notificationService;
        _listComponent = new NotificationListComponent(_config, _notificationService);

        _listComponent.on('error', function(message){
            $element.find('.error').removeClass('hidden');
            $element.find('.error-text').html(message);
        });

        $element = $(template);
        $element.on('show.bs.dropdown', function(){
            _listComponent.updateList();
            clearTitleNitifyInterval();
        });
        $element.find('.select-all-btn').on('click', function(){
            if(this.checked){
                _listComponent.selectAllNotifications();
            } else {
                _listComponent.unSelectAllNotifications();
            }
        });
        connectToHub();
    }

    updateBadge(count){
        updateBadge(count);
    }

    updateList(){
        _listComponent.updateList();
    }

    init(selector){
        $(selector).append($element);

        $element.find('.dropdown-menu').on('click', function(e){
            e.stopPropagation();
        });
        $element.find('.refresh-btn').on('click', function (e) {
            e.stopPropagation();
            _listComponent.updateList();
        });
        $element.find('.mark-as-read-selected-btn').on('click', function(e){
            e.preventDefault();
            _listComponent.sendSelectedAsRead();
        });

        let submenu = $element.find('.dropdown-submenu a.dropdown-submenu-toggle');
        submenu.on('click', function(e){
            $(this).next('ul').toggle();
            e.stopPropagation();
            e.preventDefault();
        });

        $element.find('input[name="group"]').on('change', function(){

            $element.find('input[name="group"]').not(this).prop('checked', false);
            if(this.checked){
                _listComponent.groupList(this.value);
            } else {
                _listComponent.groupList(null);
            }
            submenu.next('ul').toggle();
        });

        this.updateBadge(0);
        _listComponent.init($element.find('.dropdown-menu .notify-list'));
        updateNotificationCount();
    }
}

export default MainButtonController;


