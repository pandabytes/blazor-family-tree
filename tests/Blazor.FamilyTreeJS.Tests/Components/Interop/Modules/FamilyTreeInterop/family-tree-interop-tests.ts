import * as assert from 'assert';
import * as sinon from 'sinon';
import { describe, it } from 'mocha';
import { FamilyTreeJsInteropObj } from '../../../../../../src/Components/Interop/Modules/FamilyTreeInterop/family-tree-interop';

// This is the id defined in dom-mock.js
const TreeId = 'tree';

describe('family-tree-interop-tests', () => {
  beforeEach(() => {
    FamilyTreeJsInteropObj.setupFamilyTree(TreeId);
    const familyTree = FamilyTreeJsInteropObj.getFamilyTree(TreeId);
    sinon.stub(familyTree, 'destroy');
  });

  afterEach(() => {
    FamilyTreeJsInteropObj.destroyTree(TreeId);
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
});
