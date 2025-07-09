import { IBladeEvent, IParentCallArgs } from "@vc-shell/framework";
export interface Props {
    expanded?: boolean;
    closable?: boolean;
    param?: string;
    options?: Record<string, unknown>;
}
export interface Emits {
    (event: "parent:call", args: IParentCallArgs): void;
    (event: "collapse:blade"): void;
    (event: "expand:blade"): void;
    (event: "open:blade", blade: IBladeEvent): void;
    (event: "close:blade"): void;
}
declare const _default: import("vue").DefineComponent<Props, {
    title: import("vue").ComputedRef<string>;
}, {}, {}, {}, import("vue").ComponentOptionsMixin, import("vue").ComponentOptionsMixin, {} & {
    "parent:call": (args: IParentCallArgs) => any;
    "collapse:blade": () => any;
    "expand:blade": () => any;
    "close:blade": () => any;
    "open:blade": (blade: IBladeEvent<import("vue").Component>) => any;
}, string, import("vue").PublicProps, Readonly<Props> & Readonly<{
    "onParent:call"?: ((args: IParentCallArgs) => any) | undefined;
    "onCollapse:blade"?: (() => any) | undefined;
    "onExpand:blade"?: (() => any) | undefined;
    "onClose:blade"?: (() => any) | undefined;
    "onOpen:blade"?: ((blade: IBladeEvent<import("vue").Component>) => any) | undefined;
}>, {
    expanded: boolean;
    closable: boolean;
}, {}, {}, {}, string, import("vue").ComponentProvideOptions, false, {}, any>;
export default _default;
//# sourceMappingURL=list.vue.d.ts.map