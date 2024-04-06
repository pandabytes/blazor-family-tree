import { InvalidArgumentError } from './errors';
import { PhotoUploadArgs, UpdateNodeArgs } from './event-args';
import FamilyTree from '@balkangraph/familytree.js';

// See https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability/call-dotnet-from-javascript?view=aspnetcore-7.0#create-javascript-object-and-data-references-to-pass-to-net
declare global {
  const DotNet: {
    createJSStreamReference(stream: ArrayBuffer | Blob): Promise<any>;
  }
}

class FamilyTreeJsInterop {
  FamilyTrees = new Map<string, FamilyTree>();

  public setupFamilyTree(treeId: string, options?: FamilyTree.options) {
    let familyTree = this.FamilyTrees.get(treeId);
    if (familyTree) {
      return;
    }

    familyTree = new FamilyTree(`#${treeId}`, options);
    this.FamilyTrees.set(treeId, familyTree);
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
    const familyTree = this.getFamilyTree(treeId);
    familyTree.replaceIds(oldNewIdMappings);
  }

  public registerDefaultFirstNodeHandler(treeId: string, handler: () => Object) {
    const familyTree = this.getFamilyTree(treeId);

    // Currently the typescript definition does not
    // include the event 'add', hence we need to
    // cast it to "any"
    const familyTreeObj = familyTree as any;

    familyTreeObj.on('add', (_sender: FamilyTree, _args: any) => {
      const node = handler();
      familyTree.addNode(node, undefined, false);
      return false;
    });
  }

  public registerUpdateNodeHandler(treeId: string, updateNodeHandler: (args: UpdateNodeArgs) => void) {
    const familyTree = this.getFamilyTree(treeId);

    familyTree.onUpdateNode(updateNodeHandler);
  }

  public registerPhotoUploadHandler(treeId: string, photoUploadHandler: (args: PhotoUploadArgs) => Promise<string>) {
    const familyTree = this.getFamilyTree(treeId);

    // https://www.reddit.com/r/learnjavascript/comments/fixrf1/why_is_this_lost_when_a_method_is_used_as_a/
    const familyTreeInterop = this;

    familyTree.editUI.on("element-btn-click", (sender, args) => {     
      FamilyTree.fileUploadDialog(function (file: File) {
        familyTreeInterop.uploadPhotoAsync(familyTree, file, photoUploadHandler);
      })
    });
  }

  public destroyTree(treeId: string): void {
    const familyTree = this.getFamilyTree(treeId);

    // Destroy will remove all registered events
    // associated to this family tree object
    familyTree.destroy();
    this.FamilyTrees.delete(treeId);
  }

  private async uploadPhotoAsync(
    familyTree: FamilyTree,
    file: File,
    photoUploadFunc: (args: PhotoUploadArgs) => Promise<string>
  ): Promise<void> {
    const bufferArray = await file.arrayBuffer();
    const fileStreamReference = await DotNet.createJSStreamReference(bufferArray);

    const args: PhotoUploadArgs = { fileName: file.name, fileStreamReference };
    const url = await photoUploadFunc(args);

    // Falsy value indicates handler did not do upload successfully
    if (url) {
      // Once we get back an URL, we then set it in the photo textbox html element
      const photoLabelElement = this.findPhotoLabel(familyTree);
      photoLabelElement?.classList.toggle('hasval', true);

      const photoInputElement = document.querySelector('input[data-binding="photo"');
      photoInputElement?.setAttribute('value', url);
    }
  }

  private findPhotoLabel(familyTree: FamilyTree): HTMLLabelElement | undefined {
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
    for (const htmlElement of htmlLabelElements) {
      if (hasPhoto(htmlElement.textContent)) {
        return htmlElement;
      }
    }
  }

  private getFamilyTree(treeId: string): FamilyTree {
    const familyTree = this.FamilyTrees.get(treeId);
    if (!familyTree) {
      throw new InvalidArgumentError(`Tree "${treeId}" not found.`);
    }
    return familyTree;
  }
}

export const FamilyTreeJsInteropObj = new FamilyTreeJsInterop();
