
/**
 * Ensures that the markdown is rendered when the document is ready.
 */
 /*$(function() {
  renderMarkdown();
});*/

/**
 * Finds all markdown tags and renders the file given in the "src" attribute. 
 */
function renderMarkdown(){
  $("markdown").each(function(index){
    renderElement(this);
  });
}

function renderElement(element){
    var file = element.attributes["src"].value;
    $.get(file, function(data){
      $(element).html(marked(data));        
      $('#toc').toc({container:'markdown'}); 
      var elements = $('pre code');
      $('pre code').each(function(i, e) {hljs.highlightBlock(e)});
    });
}


function renderElement2(element, showTableofContents){
    var file = element.attributes["src"].value;
    $.get(file, function(data){
      $(element).html(marked(data));    
      if (showTableofContents)    
      {
        $('#toc').toc({container:element});   
      }      
      var elements = $('pre code');
      $('pre code').each(function(i, e) {hljs.highlightBlock(e)});
    });  
}

function renderMarkdown2(showTableofContents){
  
 

  if (showTableofContents)
  {
    $("#contentWithoutToc").hide();
    $("#contentWithToc").show();
    var e = $("#contentWithToc markdown")[0];
    renderElement2(e, true);

  }
  else
  {
    $("#contentWithoutToc").show();
    $("#contentWithToc").hide();
     var e = $("#contentWithoutToc markdown")[0];
    renderElement2(e, false);
  }  
}
