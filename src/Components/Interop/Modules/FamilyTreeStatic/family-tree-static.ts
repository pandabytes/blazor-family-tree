import FamilyTree from '@balkangraph/familytree.js';

export function setFamilyClinkCurve(curveSize: number) {
  FamilyTree.CLINK_CURVE = curveSize;
}

export const FamilyTreeObj = FamilyTree;
