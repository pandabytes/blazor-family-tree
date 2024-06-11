const jsdom = require('jsdom');
const sinon = require('sinon');

// Globally mock the DOM to have the div for
// the FamilyTreeJS library to "render"
const dom = new jsdom.JSDOM(`
  <div id="tree"></div>
  <div id="other-tree"></div>
`);
global.document = dom.window.document;
global.XMLHttpRequest = sinon.useFakeXMLHttpRequest();
