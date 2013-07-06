﻿function ChatRoomName(opt) {
    var options = {
        groupID: 0,
        roomNameTemplate: "",
        contentClientID: "",
        clientID: "",
        conversationTitle: "",
        noRoomTitle: "No room",
        displayInitialTitle: true,
        loadingDiv: "",
        envelopeID: ""
    };
    
    this.Options = jQuery.extend(options, opt);
    this.Type = "ChatRoomName";

    // Build jQuery template
    var chatRoomNameTemplateB = "chatRoomNameTemplateB_" + options.clientID,
        envelope = jQuery(options.envelopeID),
        content = jQuery(options.contentClientID);
        
    jQuery.template(chatRoomNameTemplateB, options.roomNameTemplate);


    this.Clear = function() {
        if ((ChatManager.Login.IsLoggedIn) && (options.displayInitialTitle == true)) {
            content.empty();
            jQuery.tmpl(chatRoomNameTemplateB, { RoomName: options.noRoomTitle }).appendTo(content);
            envelope.show();
        }
        else {
            envelope.hide();
        }
    };


    this.ProcessResponse = function(room) {
        content.empty();
        envelope.show();

        var name = (room.WhisperUserID == null) ? room.DisplayName : options.conversationTitle;
        jQuery.tmpl(chatRoomNameTemplateB, { RoomName: name }).appendTo(content);
    };
    

    this.ShowLoading = function(show) {
        if (show) {
            content.html(options.loadingDiv);
        }
        else {
            content.find(".ChatRoomNameWPLoading").remove();
        }
    };

    this.Clear();
};

function InitChatRoomNameWebpart(opt) {
    InicializeChatManager();
    ChatManager.RegisterWebpart(new ChatRoomName(opt));
};