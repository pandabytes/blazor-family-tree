import * as assert from 'assert';
import * as sinon from 'sinon';
import * as chai from 'chai';
import { describe, it } from 'mocha';
import { FamilyTreeJsInteropObj } from '../../../../../../src/Components/Interop/Modules/FamilyTreeInterop/family-tree-interop';
import FamilyTree from '@balkangraph/familytree.js';

// These ids are defined in dom-mock.js
const TreeId = 'tree';
const OtherTreeId = 'other-tree';

describe('family-tree-interop-tests', () => {
  beforeEach(() => {
    FamilyTreeJsInteropObj.setupFamilyTree(TreeId);
    FamilyTreeJsInteropObj.setupFamilyTree(OtherTreeId);

    const familyTree = FamilyTreeJsInteropObj.getFamilyTree(TreeId);
    const otherFamilyTree = FamilyTreeJsInteropObj.getFamilyTree(OtherTreeId);

    sinon.stub(familyTree, 'destroy');
    sinon.stub(otherFamilyTree, 'destroy');
  });

  afterEach(() => {
    FamilyTreeJsInteropObj.destroyTree(TreeId);
    FamilyTreeJsInteropObj.destroyTree(OtherTreeId);
    sinon.restore();
  });

  describe('treeExist', () => {
    it('returns false when tree id does not exist', () => {
      assert.equal(false, FamilyTreeJsInteropObj.treeExist('x'));
    });

    it('returns true when tree id does exist', () => {
      // Act
      const treeExists = FamilyTreeJsInteropObj.treeExist(TreeId);
      
      // Assert
      assert.equal(true, treeExists);
    });
  });

  describe('getFamilyTree', () => {
    it('throws excpetion when FamilyTree not found', () => {
      assert.throws(() => FamilyTreeJsInteropObj.getFamilyTree('x'));
    });

    it('returns FamilyTree when found', () => {
      // Act
      const familyTree = FamilyTreeJsInteropObj.getFamilyTree(TreeId);
      
      // Assert
      assert.notEqual(familyTree, null);
      assert.notEqual(familyTree, undefined);
    });
  });

  describe('setupFamilyTree', () => {
    it('does nothing when FamilyTree already exists', () => {
      // Act
      FamilyTreeJsInteropObj.setupFamilyTree(TreeId);

      // Assert
      const familyTree = FamilyTreeJsInteropObj.getFamilyTree(TreeId);
      assert.notEqual(familyTree, null);
      assert.notEqual(familyTree, undefined);
    });
  });

  describe('loadNodes', () => {
    it('calls FamilyTree.load once', () => {
      // Arrange
      const familyTree = FamilyTreeJsInteropObj.getFamilyTree(TreeId);
      const loadStub = sinon.stub(familyTree, 'load');

      // Act
      FamilyTreeJsInteropObj.loadNodes(TreeId, [
        { id: '1', name: 'John' },
        { id: '2', name: 'Jane' }
      ]);

      // Assert
      sinon.assert.calledOnce(loadStub);
    });
  });

  describe('removeNode', () => {
    it('removes node when possible', () => {
      // Arrange
      const familyTree = FamilyTreeJsInteropObj.getFamilyTree(TreeId);
      const canRemoveStub = sinon.stub(familyTree, 'canRemove').returns(true);
      const removeNodeStub = sinon.stub(familyTree, 'removeNode');

      // Act
      const removed = FamilyTreeJsInteropObj.removeNode(TreeId, '1');

      // Assert
      assert.equal(true, removed);
      sinon.assert.calledOnce(canRemoveStub);
      sinon.assert.calledOnce(removeNodeStub);
    });

    it('does not remove node when not possible', () => {
      // Arrange
      const familyTree = FamilyTreeJsInteropObj.getFamilyTree(TreeId);
      const canRemoveStub = sinon.stub(familyTree, 'canRemove').returns(false);
      const removeNodeStub = sinon.stub(familyTree, 'removeNode');

      // Act
      const removed = FamilyTreeJsInteropObj.removeNode(TreeId, '1');

      // Assert
      assert.equal(false, removed);
      sinon.assert.calledOnce(canRemoveStub);
      sinon.assert.notCalled(removeNodeStub);
    });
  });

  describe('addCustomInputElement', () => {
    it('adds non-existing input type', () => {
      // Arrange
      const inputType = 'customInput';
      const inputCallbackStub = (
        _data: FamilyTree.node, _editElement: FamilyTree.editFormElement,
        _minWidth: string, _readOnly: boolean
      ) => { return { html: ''} };

      // Act
      FamilyTreeJsInteropObj.addCustomInputElement(TreeId, inputType, inputCallbackStub);

      // Assert
      const customInputElements = FamilyTreeJsInteropObj.getFamilyTreeWrapper(TreeId).customInputElements;
      chai.should().exist(customInputElements);
      chai.expect(customInputElements.has(inputType)).to.be.equal(true);
    });

    it('adds existing input type throws exception', () => {
      // Arrange
      const inputType = 'customInput';
      const inputCallbackStub = (
        _data: FamilyTree.node, _editElement: FamilyTree.editFormElement,
        _minWidth: string, _readOnly: boolean
      ) => { return { html: ''} };
      FamilyTreeJsInteropObj.addCustomInputElement(TreeId, inputType, inputCallbackStub);

      // Act & Assert
      assert.throws(() => {
        FamilyTreeJsInteropObj.addCustomInputElement(TreeId, inputType, inputCallbackStub);
      });
    });

    it('adds existing input type throws exception regardless of tree id', () => {
      // Arrange
      const inputType = 'customInput';
      const inputCallbackStub = (
        _data: FamilyTree.node, _editElement: FamilyTree.editFormElement,
        _minWidth: string, _readOnly: boolean
      ) => { return { html: ''} };

      FamilyTreeJsInteropObj.addCustomInputElement(TreeId, inputType, inputCallbackStub);

      // Act & Assert
      assert.throws(() => {
        FamilyTreeJsInteropObj.addCustomInputElement(OtherTreeId, inputType, inputCallbackStub);
      });
    });
  });
});
