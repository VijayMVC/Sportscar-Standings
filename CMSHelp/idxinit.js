/*!
Index page init functions for Premium Pack Version 1.51 for Help & Manual 6
Copyright (c) 2008-2011 by Tim Green. 
All rights reserved. 
*/
var IE6=/msie 6|MSIE 6/.test(navigator.userAgent);var clicked=[];var idxTMP="";var idxCount=0;if(!IE6){addEvent(window,"load",headResizeIDX);addEvent(window,"resize",headResizeIDX);}function idxPageInit(){$("a[href*='#']").not("a[href*='javascript'], li#current a, div#current a").click(function(b){b.preventDefault();var c=Math.floor(Math.random()*10001);while($.inArray(c,window.clicked)>-1){c=Math.floor(Math.random()*10001);}window.clicked.push(c);var a=$(this).attr("href");a=a.replace("#","?xx="+c+"#");parent.hmcontent.location=a;});$("#idxSearch").keydown(function(a){if(a.keyCode==13){a.preventDefault();hmGoIndex();}});$.scrollTo({top:"0px",left:"0px"},400);}$(document).ready(function(){idxPageInit();});function gotoIndex(c){var d=document.getElementById(window.headElement).offsetHeight;var a=document.getElementById(c.charAt(1));var b=document.getElementsByName(c.charAt(1));var e=b[0];if(a){$.scrollTo(a,400,{offset:-d});}else{if(e){$.scrollTo(e,400,{offset:-d});}else{callouttext=idxCallout+"&nbsp;"+c.charAt(1);document.getElementById("callout").innerHTML=callouttext;document.getElementById("callout-wrap").style.display="block";setTimeout("document.getElementById('callout-wrap').style.display='none'",2500);}}}function idxIncr(a){if((idxTMP==a)&&(a!="")){idxCount++;}else{idxCount=0;idxTMP=a;}}function hmGoIndex(){var a=$("#idxSearch").attr("value");idxIncr(a);if(a==""){hmFindKey();}if(idxCount>0&&a!=""){hmScrollToKey();}else{hmFindKey();}}function hmFindKey(){var c=($("#"+window.headElement).height()+20)*-1;var a=$("#idxSearch").attr("value");$("span.idxkeyword,span.idxkeyword2").each(function(e){$(this).css("backgroundColor","transparent");});var b=$("span.idxkeyword,span.idxkeyword2").filter(function(){return(new RegExp(a,"i")).test($(this).text());});var d=$(b)[0];if(b.length>0&&a.length>0){$(b).each(function(e){$(this).css("background-color",window.idxHl);});$.scrollTo(d,400,{offset:c});}else{if(a.length==0){$.scrollTo({top:"0px",left:"0px"},400);}}}function hmScrollToKey(){var d=($("#"+window.headElement).height()+20)*-1;var c=$(window).height();var a=$("#idxSearch").attr("value");var b=$("span.idxkeyword,span.idxkeyword2").filter(function(){return(new RegExp(a,"i")).test($(this).text());});if(idxCount<b.length&&a!=""){var e=$(b[idxCount]).offset();while(e.top<(c+$(window).scrollTop())){idxCount++;e=$(b[idxCount]).offset();}$.scrollTo(b[idxCount],400,{offset:d});}}