
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
  
  function renderElement(element){
    var file = element.attributes["src"].value;
    $.get(file, function(data){
      $(element).html(marked(data));        
      $('#toc').toc({container:'markdown'}); 
      var elements = $('pre code');
      $('pre code').each(function(i, e) {hljs.highlightBlock(e)});
    });
  }
}


