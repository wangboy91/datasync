;(function(){
  var state={
    sources:[
      {id:'src_mysql',name:'MySQL 源',type:'MySQL',status:'已连接'},
      {id:'src_pg',name:'Postgres 源',type:'PostgreSQL',status:'已连接'}
    ],
    targets:[
      {id:'tgt_pg',name:'Postgres 目标',type:'PostgreSQL',status:'已连接'}
    ],
    rules:[
      {id:'rule_users',name:'用户表同步',source:{id:'src_mysql',table:'users'},target:{id:'tgt_pg',table:'users'},mode:'增量',dedupeKeys:['email'],mergePolicy:{id:'skip',email:'update',name:'update',created_at:'skip'}}
    ],
    jobs:[
      {rule:'用户表同步',trigger:'手动',start:'2025-12-28 10:12',end:'2025-12-28 10:14',status:'成功',count:1200},
      {rule:'订单表同步',trigger:'接口',start:'2025-12-27 21:02',end:'2025-12-27 21:08',status:'失败',count:540}
    ],
    schema:{
      src_mysql:{
        default:{
          users:[
            {name:'id',type:'INT',nullable:false,default:null,index:'PK'},
            {name:'email',type:'VARCHAR(255)',nullable:false,default:null,index:'UNIQUE'},
            {name:'name',type:'VARCHAR(100)',nullable:true,default:null,index:null},
            {name:'created_at',type:'DATETIME',nullable:false,default:'CURRENT_TIMESTAMP',index:null}
          ],
          orders:[
            {name:'id',type:'INT',nullable:false,default:null,index:'PK'},
            {name:'user_id',type:'INT',nullable:false,default:null,index:'INDEX'},
            {name:'amount',type:'DECIMAL(10,2)',nullable:false,default:'0',index:null},
            {name:'status',type:'VARCHAR(32)',nullable:false,default:'pending',index:'INDEX'}
          ]
        }
      },
      src_pg:{
        public:{
          users:[
            {name:'id',type:'INT',nullable:false,default:null,index:'PK'},
            {name:'email',type:'TEXT',nullable:false,default:null,index:'UNIQUE'},
            {name:'name',type:'TEXT',nullable:true,default:null,index:null},
            {name:'created_at',type:'TIMESTAMP',nullable:false,default:'now()',index:null}
          ]
        }
      }
    },
    targetSchema:{
      tgt_pg:{
        public:{
          users:[
            {name:'id',type:'INT',nullable:false,default:null,index:'PK'},
            {name:'email',type:'TEXT',nullable:false,default:null,index:'UNIQUE'},
            {name:'name',type:'TEXT',nullable:true,default:null,index:null},
            {name:'created_at',type:'TIMESTAMP',nullable:false,default:'now()',index:null}
          ],
          orders:[
            {name:'id',type:'INT',nullable:false,default:null,index:'PK'},
            {name:'user_id',type:'INT',nullable:false,default:null,index:'INDEX'},
            {name:'amount',type:'DECIMAL(10,2)',nullable:false,default:'0',index:null},
            {name:'status',type:'TEXT',nullable:false,default:'pending',index:'INDEX'}
          ]
        }
      }
    },
    triggerUrl:'http://localhost:8000/api/sync/rule_users?token=example'
  }
  function byId(id){return document.getElementById(id)}
  function text(el,t){if(el){el.textContent=t}}
  function renderIndex(){
    text(byId('metric-sources'),state.sources.length)
    text(byId('metric-targets'),state.targets.length)
    text(byId('metric-rules'),state.rules.length)
    text(byId('metric-last'),state.jobs.length?state.jobs[0].status:'—')
    var tbody=byId('recent-jobs')?byId('recent-jobs').querySelector('tbody'):null
    if(!tbody)return
    tbody.innerHTML=''
    state.jobs.forEach(function(j){
      var tr=document.createElement('tr')
      var statusClass=j.status==='成功'?'status-success':(j.status==='失败'?'status-failed':'status-running')
      tr.innerHTML='<td>'+j.rule+'</td><td>'+j.trigger+'</td><td>'+j.start+'</td><td>'+j.end+'</td><td><span class="status-badge '+statusClass+'">'+j.status+'</span></td><td>'+j.count+'</td><td><a class="btn secondary" href="pages/jobs.html">查看</a></td>'
      tbody.appendChild(tr)
    })
  }
  function renderRulesList(){
    var tbody=document.getElementById('rules-list-tbody')
    if(!tbody)return
    tbody.innerHTML=''
    state.rules.forEach(function(r){
      var dedupe=r.dedupeKeys&&r.dedupeKeys.length?r.dedupeKeys.join(', '):'—'
      var mp=r.mergePolicy||{}
      var updateCnt=Object.keys(mp).filter(function(k){return mp[k]==='update'}).length
      var skipCnt=Object.keys(mp).filter(function(k){return mp[k]==='skip'}).length
      var tr=document.createElement('tr')
      tr.innerHTML='<td>'+r.name+'</td><td>'+r.source.id+' / '+r.source.table+'</td><td>'+r.target.id+' / '+r.target.table+'</td><td>'+r.mode+'</td><td>'+dedupe+'</td><td>更新 '+updateCnt+' · 跳过 '+skipCnt+'</td><td><span class="status-badge status-success">—</span></td><td class="table-actions"><a class="btn secondary" href="rules.html?rule_id='+r.id+'">编辑</a><a class="btn secondary" style="margin-left:8px" href="execute.html">执行</a></td>'
      tbody.appendChild(tr)
    })
  }
  function bindTest(btnId,outId){
    var btn=byId(btnId),out=byId(outId)
    if(btn&&out){
      btn.addEventListener('click',function(){
        btn.disabled=true
        setTimeout(function(){
          btn.disabled=false
          out.textContent='连接成功'
          out.className='status-badge status-success'
        },600)
      })
    }
  }
  function renderSources(){
    bindTest('btn-test-source','test-source-result')
    var list=byId('source-list')
    if(list){
      list.innerHTML=''
      state.sources.forEach(function(s){
        var li=document.createElement('li')
        li.className='panel'
        li.innerHTML='<div class="panel-title">'+s.name+' · '+s.type+'</div><div><span class="status-badge status-success">'+s.status+'</span></div>'
        list.appendChild(li)
      })
    }
  }
  function renderTargets(){
    bindTest('btn-test-target','test-target-result')
    var list=byId('target-list')
    if(list){
      list.innerHTML=''
      state.targets.forEach(function(t){
        var li=document.createElement('li')
        li.className='panel'
        li.innerHTML='<div class="panel-title">'+t.name+' · '+t.type+'</div><div><span class="status-badge status-success">'+t.status+'</span></div>'
        list.appendChild(li)
      })
    }
  }
  function renderSchema(){
    var srcSel=byId('schema-source'),schSel=byId('schema-name'),tblSel=byId('schema-table'),tbody=byId('schema-tbody')
    if(!srcSel||!schSel||!tblSel||!tbody)return
    srcSel.innerHTML='<option value="src_mysql">MySQL 源</option><option value="src_pg">Postgres 源</option>'
    function updateSchemas(){
      var v=srcSel.value
      var schemas=Object.keys(state.schema[v]||{})
      schSel.innerHTML=schemas.map(function(s){return '<option>'+s+'</option>'}).join('')
      updateTables()
    }
    function updateTables(){
      var v=srcSel.value
      var sch=schSel.value
      var tables=Object.keys(((state.schema[v]||{})[sch])||{})
      tblSel.innerHTML=tables.map(function(t){return '<option>'+t+'</option>'}).join('')
      renderTable()
    }
    function renderTable(){
      var v=srcSel.value,sch=schSel.value,tbl=tblSel.value
      var rows=((state.schema[v]||{})[sch]||{})[tbl]||[]
      tbody.innerHTML=''
      rows.forEach(function(r){
        var tr=document.createElement('tr')
        tr.innerHTML='<td>'+r.name+'</td><td>'+r.type+'</td><td>'+(r.nullable?'是':'否')+'</td><td>'+(r.default===null?'':r.default)+'</td><td>'+(r.index||'')+'</td>'
        tbody.appendChild(tr)
      })
    }
    srcSel.onchange=updateSchemas
    schSel.onchange=updateTables
    tblSel.onchange=renderTable
    updateSchemas()
  }
  function renderRules(){
    var src=byId('rule-source'),srctbl=byId('rule-source-table'),tgt=byId('rule-target'),tgttbl=byId('rule-target-table'),mapTbody=byId('map-tbody')
    var mergeTbody=byId('merge-tbody'),conflictSel=byId('conflict-mode'),saveBtn=byId('btn-save-rule')
    if(!src||!srctbl||!tgt||!tgttbl||!mapTbody)return
    src.innerHTML='<option value="src_mysql">MySQL 源</option><option value="src_pg">Postgres 源</option>'
    tgt.innerHTML='<option value="tgt_pg">Postgres 目标</option>'
    function updateSourceTables(){
      var v=src.value
      var schemas=Object.keys(state.schema[v]||{})
      var first=schemas[0]
      var tables=Object.keys(((state.schema[v]||{})[first])||{})
      srctbl.innerHTML=tables.map(function(t){return '<option>'+t+'</option>'}).join('')
      renderMapping()
    }
    function updateTargetTables(){
      tgttbl.innerHTML='<option>users</option><option>orders</option>'
      renderMapping()
      renderDedupeAndMerge()
    }
    function renderMapping(){
      var v=src.value
      var schemas=Object.keys(state.schema[v]||{})
      var sch=schemas[0]
      var rows=((state.schema[v]||{})[sch]||{})[srctbl.value]||[]
      mapTbody.innerHTML=''
      rows.forEach(function(r){
        var tr=document.createElement('tr')
        tr.innerHTML='<td>'+r.name+'</td><td><select class="select target-select"><option value="'+r.name+'">'+r.name+'</option></select></td><td><select class="select"><option>无</option><option>trim</option><option>lower</option><option>toInt</option></select></td><td><input class="input" placeholder="默认值"></td><td><label style="display:inline-flex;align-items:center;gap:6px"><input type="checkbox" class="dedupe-box"><span>作为验重</span></label></td>'
        mapTbody.appendChild(tr)
      })
    }
    function getTargetFields(){
      var fields=((state.targetSchema['tgt_pg']||{}).public||{})[tgttbl.value]||[]
      return fields.map(function(f){return f.name})
    }
    function renderDedupeAndMerge(){
      if(!mergeTbody)return
      var fields=getTargetFields()
      mergeTbody.innerHTML=''
      fields.forEach(function(f){
        var tr=document.createElement('tr')
        tr.innerHTML='<td>'+f+'</td><td><select class="select"><option value="update">更新</option><option value="skip">跳过</option><option value="updateIfEmpty">仅空时更新</option></select></td>'
        mergeTbody.appendChild(tr)
      })
      toggleMergeVisibility()
    }
    function toggleMergeVisibility(){
      var section=document.getElementById('merge-tbody')?.closest('section')
      if(!conflictSel||!section)return
      section.style.display=conflictSel.value==='合并'?'block':'block'  // 原型默认显示；可切换为隐藏
    }
    function prefillIfEditing(){
      var params=new URLSearchParams(location.search)
      var id=params.get('rule_id')
      if(!id)return
      var rule=state.rules.find(function(r){return r.id===id})
      if(!rule)return
      src.value=rule.source.id
      updateSourceTables()
      srctbl.value=rule.source.table
      tgt.value=rule.target.id
      updateTargetTables()
      tgttbl.value=rule.target.table
      renderDedupeAndMerge()
      if(rule.dedupeKeys){
        var rows=mapTbody.querySelectorAll('tr')
        rows.forEach(function(row){
          var sel=row.querySelector('select.target-select')
          var box=row.querySelector('input.dedupe-box')
          if(sel&&box){ box.checked=rule.dedupeKeys.indexOf(sel.value)>=0 }
        })
      }
      if(mergeTbody&&rule.mergePolicy){
        var rows=mergeTbody.querySelectorAll('tr')
        rows.forEach(function(row){
          var field=row.children[0].textContent
          var sel=row.children[1].querySelector('select')
          if(rule.mergePolicy[field]) sel.value=rule.mergePolicy[field]
        })
      }
    }
    function collectAndSave(){
      var dedupe=[]
      var rows=mapTbody.querySelectorAll('tr')
      rows.forEach(function(row){
        var sel=row.querySelector('select.target-select')
        var box=row.querySelector('input.dedupe-box')
        if(sel&&box&&box.checked){ dedupe.push(sel.value) }
      })
      var merge={}
      if(mergeTbody){
        mergeTbody.querySelectorAll('tr').forEach(function(row){
          var field=row.children[0].textContent
          var sel=row.children[1].querySelector('select')
          merge[field]=sel.value
        })
      }
      var name='新规则 '+Date.now()
      var newRule={id:'rule_'+Date.now(),name:name,source:{id:src.value,table:srctbl.value},target:{id:tgt.value,table:tgttbl.value},mode:'增量',dedupeKeys:dedupe,mergePolicy:merge}
      state.rules.push(newRule)
      location.href='rules-list.html'
    }
    srctbl.onchange=renderMapping
    tgttbl.onchange=renderMapping
    if(conflictSel) conflictSel.onchange=toggleMergeVisibility
    if(saveBtn) saveBtn.addEventListener('click',collectAndSave)
    updateSourceTables()
    updateTargetTables()
    prefillIfEditing()
  }
  function renderDedupe(){
    var type=byId('ddp-type'),expr=byId('ddp-expr')
    if(!type||!expr)return
    type.onchange=function(){
      expr.placeholder=type.value==='表达式'?'例如 email+phone':'例如 id 或 email'
    }
  }
  function renderExecute(){
    var urlEl=byId('trigger-url'),curlEl=byId('curl')
    var progress=byId('progress-bar')
    var runBtn=byId('btn-run')
    var ruleSel=byId('exec-rule')
    if(urlEl)urlEl.textContent=state.triggerUrl
    if(curlEl)curlEl.textContent='curl -X POST \"'+state.triggerUrl+'\"'
    if(ruleSel){
      ruleSel.innerHTML=state.rules.map(function(r){return '<option value="'+r.id+'">'+r.name+'</option>'}).join('')
    }
    if(runBtn&&progress){
      runBtn.addEventListener('click',function(){
        var p=0
        progress.style.width='0%'
        runBtn.disabled=true
        var timer=setInterval(function(){
          p+=10
          progress.style.width=p+'%'
          if(p>=100){clearInterval(timer);runBtn.disabled=false}
        },200)
      })
    }
  }
  function renderJobs(){
    var tbody=byId('jobs-tbody')
    if(!tbody)return
    tbody.innerHTML=''
    state.jobs.forEach(function(j){
      var statusClass=j.status==='成功'?'status-success':(j.status==='失败'?'status-failed':'status-running')
      var tr=document.createElement('tr')
      tr.innerHTML='<td>'+j.rule+'</td><td>'+j.trigger+'</td><td>'+j.start+'</td><td>'+j.end+'</td><td><span class="status-badge '+statusClass+'">'+j.status+'</span></td><td>'+j.count+'</td>'
      tbody.appendChild(tr)
    })
  }
  function renderSettings(){}
  var page=document.body.getAttribute('data-page')||''
  if(page==='index')renderIndex()
  if(page==='sources')renderSources()
  if(page==='targets')renderTargets()
  if(page==='schema')renderSchema()
  if(page==='rules')renderRules()
  if(page==='rules-list')renderRulesList()
  if(page==='dedupe')renderDedupe()
  if(page==='execute')renderExecute()
  if(page==='jobs')renderJobs()
  if(page==='settings')renderSettings()
}())
