// We import DotNet namespace like this just to get the type declaration
// but we don't want to bundle it in our output .js file. Because the
// namespace DotNet is already available in the browser when Blazor
// loads the app
// Idea taken from here https://stackoverflow.com/questions/74723484/how-to-get-vite-to-not-import-bundle-an-external-dependency/74723564
import type { DotNet as NpmDotNet } from '@microsoft/dotnet-js-interop';

import { InvalidArgumentError } from './errors';
import FamilyTree from '@balkangraph/familytree.js';

declare global {
  const DotNet: typeof NpmDotNet;
}

/*
  These types & their properties must match
  with the classes and properties defined in C# code.
*/
type UpdateNodeArgs = {
  addNodesData: Array<object>,
  updateNodesData: Array<object>,
  removeNodeId: number | string
}

type PhotoUploadArgs = {
  fileName: string,
  fileStreamReference: any
}

type TextboxButtonClickedArgs = {
  nodeId: string | number;
  bindingName?: string;
  buttonText?: string;
}

type InputElementCallback = (
  data: FamilyTree.node, editElement: FamilyTree.editFormElement,
  minWidth: string, readOnly: boolean
) => { html: string, id?: number | string, value?: any };

type FamilyTreeWrapper = {
  instance: FamilyTree;
  customInputElements?: Map<string, InputElementCallback>;
}

FamilyTree.elements.xxxreadOnlyTextBox = (
  data: FamilyTree.node, editElement: FamilyTree.editFormElement,
  minWidth: string, readOnly: boolean
) => {
  const id = data['id']
  console.log(data);
  console.log(editElement);
  console.log(readOnly);
  // Force the "edit" page to render the field as readonly
  // This means the field is always readonly
  const value = data[editElement.binding];  

  if (readOnly && !value) {
    alert('readonly with no value');
    return { html: '' };
  }

  const html = (readOnly) ? `
    <div class="bft-input" data-bft-input="" data-bft-input-disabled="">
      <label for="${id}" class="hasval">${editElement.label}</label>
      <input readonly data-binding="${editElement.binding}" maxlength="256" id="${id}" name="${id}" type="text" value="${value}" autocomplete="off">
    </div>
  ` : `
    <div class="bft-form-field" style="min-width: ${minWidth};">    
      <div class="bft-input" data-bft-input="">
        <label for="${id}" class="hasval">${editElement.label}</label>
        <input readonly disabled data-binding="${editElement.binding}" maxlength="256" id="${id}" name="${id}" type="text" value="${value}" autocomplete="off">
      </div>
    </div>
  `;

  return {
    html: html,
    id: id,
    value: value
  };
};

class FamilyTreeJsInterop {
  private familyTrees = new Map<string, FamilyTreeWrapper>();

  public treeExist(treeId: string): boolean {
    return this.familyTrees.has(treeId);
  }

  public getFamilyTree(treeId: string): FamilyTree {
    const familyTreeWrapper = this.familyTrees.get(treeId);
    if (!familyTreeWrapper) {
      throw new InvalidArgumentError(`Tree "${treeId}" not found.`);
    }
    return familyTreeWrapper.instance;
  }

  public getFamilyTreeWrapper(treeId: string): FamilyTreeWrapper {
    const familyTreeWrapper = this.familyTrees.get(treeId);
    if (!familyTreeWrapper) {
      throw new InvalidArgumentError(`Tree "${treeId}" not found.`);
    }
    return familyTreeWrapper;
  }

  public setupFamilyTree(treeId: string, options?: FamilyTree.options) {
    const familyTreeWrapper = this.familyTrees.get(treeId);
    if (familyTreeWrapper) {
      return;
    }

    const familyTree = new FamilyTree(`#${treeId}`, options);
    this.familyTrees.set(treeId, { instance: familyTree });
  }

  public loadNodes(treeId: string, nodes: Array<Object>) {
    const familyTree = this.getFamilyTree(treeId);
    familyTree.load(nodes);
  }

  public removeNode(treeId: string, nodeId: string) {
    const familyTree = this.getFamilyTree(treeId);

    const canRemove = familyTree.canRemove(nodeId);
    if (canRemove) {
      familyTree.removeNode(nodeId, undefined, true);
    }
    return canRemove;
  }

  public replaceNodeIds(treeId: string, oldNewIdMappings: { [key: string]: string | number }) {
    this.getFamilyTree(treeId).replaceIds(oldNewIdMappings);
  }

  public addCustomInputElement(treeId: string, inputType: string, inputCallback: InputElementCallback) {
    const familyTreeWrapper = this.getFamilyTreeWrapper(treeId);
    let customInputElements = familyTreeWrapper.customInputElements;

    if (!customInputElements) {
      familyTreeWrapper.customInputElements = new Map<string, InputElementCallback>();
      customInputElements = familyTreeWrapper.customInputElements
    }

    const keys = Object.keys(FamilyTree.elements);
    if (keys.includes(inputType)) {
      throw new InvalidArgumentError(`Custom element inputType "${inputType}" already existed.`);
    }

    customInputElements.set(inputType, inputCallback);
    FamilyTree.elements[inputType] = inputCallback;
  }

  /**
   * See https://github.com/BALKANGraph/FamilyTreeJS/issues/97
   * @param treeId 
   * @param handler 
   */
  public registerDefaultFirstNodeHandler(treeId: string, handler: () => Object) {
    const familyTree = this.getFamilyTree(treeId);

    // Currently the typescript definition does not
    // include the event 'add', hence we need to
    // cast it to "any"
    const familyTreeObj = familyTree as any;

    // This callback is called whenever ".addNode({...}, undefined, true)" is called
    // Since we only want to override the default node, we have to check
    // if the family tree is empty AND the node being added is the node
    // provided by the FamilyTreeJS library
    familyTreeObj.on('add', (sender: FamilyTree, arg1: any, _arg2: any, _arg3: any) => {
      if (FamilyTreeJsInterop.isFamilyTreeEmpty(sender) &&
          FamilyTreeJsInterop.isFirstDefaultNode(arg1)) {
        const node = handler();

        // Don't fire add event again because we'll be
        // stuck in recursive call forever
        sender.addNode(node, undefined, false);
  
        // Return false to cancel the default first node add
        // If we don't, it will add its default node in addition
        // to our custom default node
        return false;
      }
    });
  }

  public registerUpdateNodeHandler(treeId: string, updateNodeHandler: (args: UpdateNodeArgs) => void) {
    const familyTree = this.getFamilyTree(treeId);
    familyTree.onUpdateNode(updateNodeHandler);
  }

  public registerPhotoUploadHandler(treeId: string, photoUploadHandler: (args: PhotoUploadArgs) => Promise<string>) {
    const familyTree = this.getFamilyTree(treeId);

    familyTree.editUI.on("element-btn-click", (_sender, args) => {
      FamilyTree.fileUploadDialog(function (file: File) {
        FamilyTreeJsInterop.uploadPhotoAsync(file, args.input, photoUploadHandler);
      });
    });
  }

  public registerInputButtonClickedHandler(
    treeId: string,
    inputButtonClickedHandler: (args: TextboxButtonClickedArgs) => Promise<string>
  ) {
    const familyTree = this.getFamilyTree(treeId);

    familyTree.editUI.on('element-btn-click', (_sender, args) => {
      const inputElement: HTMLInputElement = args.input;
      const nodeId = args.nodeId;
      const buttonLinkElement = FamilyTreeJsInterop.findButtonLinkElementFromInput(inputElement);
      const textboxButtonClickedArgs : TextboxButtonClickedArgs = { 
        nodeId,
        buttonText: buttonLinkElement?.textContent,
        bindingName: inputElement.getAttribute('data-binding'),
      };

      // Call the handler and then asynchronously update the 
      // the input textbox's with the value returned from
      // the handler
      inputButtonClickedHandler(textboxButtonClickedArgs)
        .then(value => {
          const labelElement = FamilyTreeJsInterop.findLabelElementFromInput(inputElement);
          labelElement?.classList.toggle('hasval', true);
          inputElement?.setAttribute('value', value);
      });
    });
  }

  public destroyTree(treeId: string): void {
    const familyTreeWrapper = this.getFamilyTreeWrapper(treeId);
    
    // Remove custom input elements
    if (familyTreeWrapper.customInputElements) {
      familyTreeWrapper.customInputElements.forEach((_v, inputType) => {
        delete FamilyTree.elements[inputType];
      });
    }
    familyTreeWrapper.customInputElements?.clear();

    // Destroy will remove all registered events
    // associated to this family tree object
    const familyTree = familyTreeWrapper.instance;
    familyTree.destroy();

    this.familyTrees.delete(treeId);
  }

  private static async uploadPhotoAsync(
    file: File,
    inputElement: HTMLInputElement,
    photoUploadFunc: (args: PhotoUploadArgs) => Promise<string>
  ): Promise<void> {
    const bufferArray = await file.arrayBuffer();
    const fileStreamReference = await DotNet.createJSStreamReference(bufferArray);

    const args: PhotoUploadArgs = { fileName: file.name, fileStreamReference };
    const url = await photoUploadFunc(args);

    // Falsy value indicates handler did not do upload successfully
    if (url) {
      // Once we get back an URL, we then set it in the photo textbox html element
      const labelElement = FamilyTreeJsInterop.findLabelElementFromInput(inputElement);
      labelElement?.classList.toggle('hasval', true);
      inputElement?.setAttribute('value', url);
    }
  }

  private static findLabelElementFromInput(inputElement: HTMLInputElement): HTMLLabelElement | null {
    const parentElement = inputElement.parentElement;
    return parentElement.querySelector('label');
  }

  private static findButtonLinkElementFromInput(inputElement: HTMLInputElement): HTMLAnchorElement | null {
    const parentElement = inputElement.parentElement;
    return parentElement.querySelector('[data-input-btn]');
  }

  private static findPhotoLabel(familyTree: FamilyTree): HTMLLabelElement | undefined {
    const elements = familyTree.config.editForm?.elements;
    if (!elements) {
      return;
    }

    const hasPhoto = (text?: string | null) => {
      if (!text) {
        return false;
      }
      return text.toLowerCase().includes('photo');
    }

    const hasPhotoLabel = elements
      .filter(element => !Array.isArray(element))
      .map(element => (<FamilyTree.editFormElement>element).label)
      .some(hasPhoto);
    
    if (!hasPhotoLabel) {
      return;
    }

    const htmlLabelElements = document.getElementsByTagName('label');
    for (const htmlElement of <any>htmlLabelElements) {
      if (hasPhoto(htmlElement.textContent)) {
        return htmlElement;
      }
    }
  }

  private static isFamilyTreeEmpty(familyTree: FamilyTree): boolean {
    const nodes = familyTree.nodes;
    return !nodes || Object.keys(nodes).length === 0;
  }

  private static isFirstDefaultNode(node: Object): boolean {
    const keys = Object.keys(node);

    // The default first node of FamilyTreeJs has this structure
    // { id: '_wasf' } where id is an auto-generated id
    if (keys.length === 1 && keys[0] === 'id') {
      const nodeId = node['id'];
      return typeof nodeId === 'string' && nodeId.startsWith('_');
    }
    return false;
  }
}

export const FamilyTreeJsInteropObj = new FamilyTreeJsInterop();
