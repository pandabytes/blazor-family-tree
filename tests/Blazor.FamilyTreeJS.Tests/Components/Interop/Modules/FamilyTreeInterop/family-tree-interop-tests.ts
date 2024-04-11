import * as assert from 'assert';
import { describe, it } from 'mocha';
import { FamilyTreeJsInteropObj } from '../../../../../../src/Components/Interop/Modules/FamilyTreeInterop/family-tree-interop';

describe('family-tree-interop-tests', () => {
  it('dummy test', () => {
    assert.equal(1, 1);

    assert.equal(false, FamilyTreeJsInteropObj.treeExist('x'));
  });
});
